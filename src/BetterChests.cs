using Terraria.ModLoader;
using BetterChests.src.Edits;
using System.IO;

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

	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		byte id = reader.ReadByte();
		switch (id) {
			case 0:
				string msg = reader.ReadString(); // "Hello world!"
				Terraria.Main.NewText(msg);
				break;
		}
	}
}
