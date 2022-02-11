using Terraria;
using Terraria.ID;

namespace BetterChests.src.Edits;

internal static class ChestHoverEdits
{
	public static void Load()
	{
		On.Terraria.Player.TileInteractionsCheckLongDistance += CloseChestHoverUI;
		On.Terraria.Player.TileInteractionsMouseOver += OpenChestHoverUI;
	}

	private static void OpenChestHoverUI(On.Terraria.Player.orig_TileInteractionsMouseOver orig, Player self, int myX, int myY)
	{
		orig(self, myX, myY);

		switch (Main.tile[myX, myY].TileType)
		{
			case TileID.Containers:
			case TileID.Containers2:
				ContainerHover(GetMultitileChest(myX, myY));
				break;
			case TileID.PiggyBank:
				ContainerHover(Main.LocalPlayer.bank);
				break;
			case TileID.Safes:
				ContainerHover(Main.LocalPlayer.bank2);
				break;
			case TileID.DefendersForge:
				ContainerHover(Main.LocalPlayer.bank3);
				break;
			case TileID.VoidVault:
				ContainerHover(Main.LocalPlayer.bank4);
				break;
		}
	}

	private static void CloseChestHoverUI(On.Terraria.Player.orig_TileInteractionsCheckLongDistance orig, Player self, int myX, int myY)
	{
		orig(self, myX, myY);

		if (Main.tile[myX, myY].TileType is not 
			(TileID.Containers or TileID.Containers2 or TileID.PiggyBank or
			TileID.Safes or TileID.DefendersForge or TileID.VoidVault) || !self.IsInTileInteractionRange(myX, myY))
		{
			UISystem.CloseChestHoverUI();
		}
	}

	private static void ContainerHover(Chest chest)
	{
		UISystem.OpenChestHoverUI(chest);
	}

	private static Chest GetMultitileChest(int x, int y)
	{
		Tile tile = Main.tile[x, y];

		int chestX = x;
		int chestY = y;
		if (tile.TileFrameX % 36 != 0)
		{
			chestX--;
		}
		if (tile.TileFrameY % 36 != 0)
		{
			chestY--;
		}

		return Main.chest[Chest.FindChest(chestX, chestY)];
	}
}
