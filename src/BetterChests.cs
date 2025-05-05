using BetterChests.Edits;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterChests;

public class BetterChests : Mod
{
	public override void Load()
	{
		// Changes the functionality of buttons
		ChestButtonEdits.Load();
        
		// allows the user to open already opened chests
		OpenChestEdits.Load();
		
		// prevents chest access if it's owned by a different player
		ChestOwnershipEdits.Load();
		
		// allows the user to ping items
		ItemSlotPingEdit.Load();
	}


	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		MultiplayerSystem.HandlePacket(reader, whoAmI);
	}

	public override object Call(params object[] args)
	{
		if (args[0] is not string function) {
			throw new Exception($"Call Error: First parameter is {args[0]} but expected string!");
		}

		// use pattern matching when https://github.com/tModLoader/tModLoader/pull/2472 gets merged
		return function switch {
			"GetOwnershipMap" => ModContent.GetInstance<OwnershipSystem>().GetMap(),
			_ => throw new Exception($"Call Error: Function '{function}' not found!")
		};
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
