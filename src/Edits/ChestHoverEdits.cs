using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BetterChests.Edits;

internal class ChestHoverEdits : GlobalTile
{
	private static readonly OwnershipSystem OwnershipSystem = ModContent.GetInstance<OwnershipSystem>();

	public override void MouseOver(int i, int j, int type)
	{
		int chestID = BetterChests.GetChest(i, j); // ONLY USE WHEN TYPE IS Containers, Containers2 OR Dressers

		if (OwnershipSystem.IsNotOwner(chestID, Main.LocalPlayer.name, out string? owner)) {
			Main.instance.MouseText(Language.GetTextValue("Mods.BetterChests.ChestOwned", owner));
			UISystem.CloseChestHoverUI();
			return;
		}
		
		if (ModContent.GetInstance<BetterChestsConfig>().disableChestHover)
			return;

		switch (type) {
			case TileID.Containers:
			case TileID.Containers2:
			case TileID.Dressers:
				UISystem.OpenChestHoverUI(Main.chest[chestID]);
				break;
			case TileID.PiggyBank:
				UISystem.OpenChestHoverUI(Main.LocalPlayer.bank);
				break;
			case TileID.Safes:
				UISystem.OpenChestHoverUI(Main.LocalPlayer.bank2);
				break;
			case TileID.DefendersForge:
				UISystem.OpenChestHoverUI(Main.LocalPlayer.bank3);
				break;
			case TileID.VoidVault:
				UISystem.OpenChestHoverUI(Main.LocalPlayer.bank4);
				break;
			default:
				UISystem.CloseChestHoverUI();
				break;
		}

		base.MouseOver(i, j, type);
	}
}
