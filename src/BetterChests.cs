using BetterChests.Edits;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BetterChests;

public class BetterChests : Mod
{
	public override void Load()
	{
		// Changes the functionality of buttons
		ChestButtonEdits.Load();

		// Increases the maximum chest name length
		ChestNameEdits.Load();

		// allows the user to open already opened chests
		OpenChestEdits.Load();
	}

	public const byte ChestUpdatePacketID = 0;
	public const byte AddChestOwnerPacketID = 1;
	public const byte RemoveChestOwnerPacketID = 2;
	public const byte GetAllOwnersPacketID = 3;

	public static bool dontUpdateMe;
	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		byte id = reader.ReadByte();
		switch (id) {
			case ChestUpdatePacketID:
				int chest = reader.ReadInt32();
				int slot = reader.ReadInt32(); // the chest slot that was changed
				Item item = ItemIO.Receive(reader, true); // item changes

				// only apply changes when in multiplayer, inside a chest and if the update didn't come from this client
				if (Main.netMode == NetmodeID.MultiplayerClient && Main.player[Main.myPlayer].chest > -1 && Main.player[Main.myPlayer].chest == chest && !dontUpdateMe) {
					Main.chest[chest].item[slot] = item;
					OpenChestEdits.serverUpdateRecieved = true;
				}

				// transmit from Server to all clients because ItemSlot is client side
				if (Main.netMode == NetmodeID.Server) {
					var packet = GetPacket();
					packet.Write(id);
					packet.Write(chest);
					packet.Write(slot);
					ItemIO.Send(item, packet, true);
					packet.Send();
				}
				break;
			case AddChestOwnerPacketID: {
				int chestID = reader.ReadInt32();
				string playerName = reader.ReadString();
				
				ModContent.GetInstance<OwnershipSystem>().SetOwner(chestID, playerName);
				// send to other clients
				if (Main.netMode == NetmodeID.Server) {
					GetAddOwnerPacket(chestID, playerName).Send();
				}

				break;
			}
			case RemoveChestOwnerPacketID: {
				int chestID = reader.ReadInt32();

				ModContent.GetInstance<OwnershipSystem>().RemoveOwner(chestID);
				// send to other clients
				if (Main.netMode == NetmodeID.Server) {
					GetRemoveOwnerPacket(chestID).Send();
				}

				break;
			}
			case GetAllOwnersPacketID: {
				if (Main.netMode != NetmodeID.Server) {
					break;
				}
				
				int player = reader.ReadInt32();

				foreach (var kv in ModContent.GetInstance<OwnershipSystem>().GetMap()) {
					GetAddOwnerPacket(kv.Key, kv.Value).Send(player);
				}

				break;
			}
		}

		dontUpdateMe = false;
	}
	
	public static ModPacket GetAddOwnerPacket(int chest, string owner)
	{
		ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
		packet.Write(AddChestOwnerPacketID);
		packet.Write(chest);
		packet.Write(owner);
		return packet;
	}

	public static ModPacket GetRemoveOwnerPacket(int chest)
	{
		ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
		packet.Write(RemoveChestOwnerPacketID);
		packet.Write(chest);
		return packet;
	}

	public static int GetChest(int x, int y)
	{
		if (TileID.Sets.BasicChest[Main.tile[x, y].TileType]) {
			return GetMultitileChest(x, y);
		}
		
		if (Main.tile[x, y].TileType == TileID.Dressers) {
			return GetDresserChest(x, y);
		}

		return -1;
	}
	
	private static int GetMultitileChest(int x, int y)
	{
		Tile tile = Main.tile[x, y];

		int chestX = x;
		int chestY = y;
		if (tile.TileFrameX % 36 != 0) {
			chestX--;
		}
		if (tile.TileFrameY % 36 != 0) {
			chestY--;
		}

		return Chest.FindChest(chestX, chestY);
	}

	// reference: TileInteractionsCheckLongDistance(int, int)
	private static int GetDresserChest(int x, int y)
	{
		Tile tile = Main.tile[x, y];

		int chestX = x - tile.TileFrameX % 54 / 18;
		int chestY = y;

		if (tile.TileFrameY % 36 != 0) {
			chestY--;
		}
			
		return Chest.FindChest(chestX, chestY);
	}
}
