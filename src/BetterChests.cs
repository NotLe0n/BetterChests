using Terraria.ModLoader;
using BetterChests.src.Edits;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace BetterChests.src;

public class BetterChests : Mod
{
	public override void Load()
	{
		// Opens the ChestHoverUI when hovering over a container
		ChestHoverEdits.Load();

		// Changes the functionality of buttons
		ChestButtonEdits.Load();

		// Increases the maximum chest name length
		ChestNameEdits.Load();

		// allows the user to open already opened chests
		OpenChestEdits.Load();
	}

	public static bool dontUpdateMe;
	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		byte id = reader.ReadByte();
		switch (id) {
			case 0:
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
		}

		dontUpdateMe = false;
	}
}
