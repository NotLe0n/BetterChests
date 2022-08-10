using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterChests.src.Edits;

internal class ChestHoverEdits : GlobalTile
{
	public override void MouseOver(int i, int j, int type)
	{
		if (ModContent.GetInstance<BetterChestsConfig>().disableChestHover)
			return;

		switch (type) {
			case TileID.Containers:
			case TileID.Containers2:
				ContainerHover(GetMultitileChest(i, j));
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
			case TileID.Dressers:
				ContainerHover(GetDresserChest(i, j));
				break;
			default:
				UISystem.CloseChestHoverUI();
				break;
		}

		base.MouseOver(i, j, type);
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
		if (tile.TileFrameX % 36 != 0) {
			chestX--;
		}
		if (tile.TileFrameY % 36 != 0) {
			chestY--;
		}

		return Main.chest[Chest.FindChest(chestX, chestY)];
	}

	// reference: TileInteractionsCheckLongDistance(int, int)
	private static Chest GetDresserChest(int x, int y)
	{
		Tile tile = Main.tile[x, y];

		int chestX = x - tile.TileFrameX % 54 / 18;
		int chestY = y;

		if (tile.TileFrameY % 36 != 0) {
			chestY--;
		}
			
		return Main.chest[Chest.FindChest(chestX, chestY)];
	}
}
