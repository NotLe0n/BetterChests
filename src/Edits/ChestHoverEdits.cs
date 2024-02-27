using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterChests.Edits;

internal class ChestHoverEdits : GlobalTile
{
	private static readonly OwnershipSystem OwnershipSystem = ModContent.GetInstance<OwnershipSystem>();

	public override void MouseOver(int i, int j, int type)
	{
		if (ModContent.GetInstance<BetterChestsConfig>().disableChestHover)
			return;

		switch (type) {
			case TileID.Containers:
			case TileID.Containers2:
			case TileID.Dressers:
				int chest;
				if (type is TileID.Containers or TileID.Containers2) {
					chest = BetterChests.GetMultitileChest(i, j);
				}
				else {
					chest = BetterChests.GetDresserChest(i, j);
				}

				if (OwnershipSystem.IsNotOwner(chest, Main.LocalPlayer.name, out string? owner)) {
					Main.instance.MouseText("This chest is owned by " + owner);
					UISystem.CloseChestHoverUI();
					return;
				}

				UISystem.OpenChestHoverUI(Main.chest[chest]);
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
