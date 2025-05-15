using System;
using System.IO;
using Terraria.ModLoader;
using BetterChests.Edits;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace BetterChests;

public static class MultiplayerSystem
{
	private enum PacketID : byte
	{
		ChestUpdate,
		AddChestOwner,
		RemoveChestOwner,
		GetAllOwners,
		SendItemSlotPing
	}
	
	public static void HandlePacket(BinaryReader reader, int whoAmI)
	{
		byte id = reader.ReadByte();
		switch ((PacketID)id) {
			case PacketID.ChestUpdate: {
				int chest = reader.ReadInt32();
				int slot = reader.ReadInt32();
				Item item = ItemIO.Receive(reader, true);
				VectorClock vc = VectorClock.Deserialize(reader);

				if (Main.netMode == NetmodeID.MultiplayerClient && Main.player[Main.myPlayer].chest > -1 &&
				    Main.player[Main.myPlayer].chest == chest) {
					ChestSyncingSystem.ReceiveChestUpdate(chest, slot, item, vc, -1); // -1 = unknown sender
				}

				if (Main.netMode == NetmodeID.Server) {
					GetChestUpdatePacket(chest, slot, item, vc).Send();
				}
				break;
			}
			case PacketID.AddChestOwner: {
				int chestID = reader.ReadInt32();
				string playerName = reader.ReadString();
				
				ModContent.GetInstance<OwnershipSystem>().SetOwner(chestID, playerName);
				// server forwards to other clients
				if (Main.netMode == NetmodeID.Server) {
					GetAddOwnerPacket(chestID, playerName).Send();
				}

				break;
			}
			case PacketID.RemoveChestOwner: {
				int chestID = reader.ReadInt32();

				ModContent.GetInstance<OwnershipSystem>().RemoveOwner(chestID);
				// server forwards to other clients
				if (Main.netMode == NetmodeID.Server) {
					GetRemoveOwnerPacket(chestID).Send();
				}

				break;
			}
			case PacketID.GetAllOwners: {
				if (Main.netMode != NetmodeID.Server) {
					break;
				}
				
				int player = reader.ReadInt32();

				foreach (var kv in ModContent.GetInstance<OwnershipSystem>().GetMap()) {
					GetAddOwnerPacket(kv.Key, kv.Value).Send(player);
				}

				break;
			}
			case PacketID.SendItemSlotPing: {
				string playerName = reader.ReadString();
				int chestID = reader.ReadInt32();
				int slotID = reader.ReadInt32();
				Color pingColor = reader.ReadRGB();
				Item renderItem = ItemIO.Receive(reader, readStack: true);
				var ping = new Ping {
					PlayerName = playerName,
					ChestID = chestID,
					SlotIndex = slotID,
					Color = pingColor,
					Item = renderItem ?? Main.chest[chestID].item[slotID]
				};
				
				// server forwards to other clients
				if (Main.netMode == NetmodeID.Server) {
					GetItemSlotPingPacket(ping).Send();
				}
				else {
					ModContent.GetInstance<PingSystem>().AddPing(ping);
				}
				break;
			}
			default:
				throw new InvalidOperationException("Unknown PacketID");
		}
	}

	public static ModPacket GetChestUpdatePacket(int chest, int slot, Item item, VectorClock vc)
	{
		ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
		packet.Write((byte)PacketID.ChestUpdate); // message id
		packet.Write(chest); // chest id
		packet.Write(slot); // slot id
		ItemIO.Send(item, packet, true); // item data
		vc.Serialize(packet); // vector clock
		return packet;
	}
	
	public static ModPacket GetAddOwnerPacket(int chest, string owner)
	{
		ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
		packet.Write((byte)PacketID.AddChestOwner);
		packet.Write(chest);
		packet.Write(owner);
		return packet;
	}

	public static ModPacket GetRemoveOwnerPacket(int chest)
	{
		ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
		packet.Write((byte)PacketID.RemoveChestOwner);
		packet.Write(chest);
		return packet;
	}
	
	public static ModPacket GetAllOwnersPacket(int id)
	{
		ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
		packet.Write((byte)PacketID.GetAllOwners);
		packet.Write(id);
		packet.Send();
		return packet;
	}
	
	public static ModPacket GetItemSlotPingPacket(Ping ping)
	{
		ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
		packet.Write((byte)PacketID.SendItemSlotPing);
		packet.Write(ping.PlayerName);
		packet.Write(ping.ChestID);
		packet.Write(ping.SlotIndex);
		packet.WriteRGB(ping.Color);
		ItemIO.Send(ping.Item, packet, writeStack: true);
		return packet;
	}
}