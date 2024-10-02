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
        var config = ModContent.GetInstance<BetterChestsConfig>();
        bool notOwner = OwnershipSystem.IsNotOwner(chestID, Main.LocalPlayer.name, out string? owner);

		if (!ModContent.GetInstance<BetterChestsMPConfig>().disableChestOwnership && TileID.Sets.BasicChest[type] && notOwner) {
			Main.instance.MouseText(Language.GetTextValue("Mods.BetterChests.ChestOwned", owner));
			UISystem.CloseChestHoverUI();
			return;
		}
		
		if (config.disableChestHover)
			return;

        if (TileID.Sets.BasicChest[type] && chestID != -1) {
            UISystem.OpenChestHoverUI(Main.chest[chestID]);
            return;
        }
        
		switch (type) {
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
    }
}
