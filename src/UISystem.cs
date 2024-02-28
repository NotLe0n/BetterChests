using BetterChests.UIStates;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests;

[Autoload(Side = ModSide.Client)]
internal class UISystem : ModSystem
{
	private UserInterface? sortOptionsUserInterface;
	private UserInterface? confirmationUserInterface;
	private UserInterface? chestHoverUserInterface;
	private UserInterface? searchbarUserInterface;
	private UserInterface? quickstackLockInterface;

	public override void Load()
	{
		sortOptionsUserInterface = new UserInterface();
		searchbarUserInterface = new UserInterface();
		confirmationUserInterface = new UserInterface();
		chestHoverUserInterface = new UserInterface();
		quickstackLockInterface = new UserInterface();

		base.Load();
	}

	private GameTime? lastUpdateUiGameTime;
	public override void UpdateUI(GameTime gameTime)
	{
		lastUpdateUiGameTime = gameTime;

		if (sortOptionsUserInterface?.CurrentState is SortOptionsUI state) {
			if (Main.LocalPlayer.chest != -1 && state.mode == SortOptionsMode.Chest) {
				sortOptionsUserInterface.Update(gameTime);
			}

			if (Main.playerInventory && state.mode == SortOptionsMode.Inventory) {
				sortOptionsUserInterface.Update(gameTime);
			}
		}

		if (Main.LocalPlayer.chest != -1) {
			if (confirmationUserInterface?.CurrentState != null) {
				confirmationUserInterface.Update(gameTime);
			}

			if (searchbarUserInterface?.CurrentState != null && !ModContent.GetInstance<BetterChestsConfig>().disableSearchbar) {
				searchbarUserInterface.Update(gameTime);
			}

			if (quickstackLockInterface?.CurrentState != null && Main.LocalPlayer.chest > 0) {
				quickstackLockInterface.Update(gameTime);
			}
		}

		if (!Main.tile[Main.MouseWorld.ToTileCoordinates()].HasTile) {
			CloseChestHoverUI();
		}

		if (chestHoverUserInterface?.CurrentState != null && !ModContent.GetInstance<BetterChestsConfig>().disableChestHover) {
			chestHoverUserInterface.Update(gameTime);
		}
	}

	private bool DrawUI()
	{
		if (sortOptionsUserInterface?.CurrentState is SortOptionsUI state) {
			if (Main.LocalPlayer.chest != -1 && state.mode == SortOptionsMode.Chest) {
				sortOptionsUserInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);
			}

			if (Main.playerInventory && state.mode == SortOptionsMode.Inventory) {
				sortOptionsUserInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);
			}
		}

		if (Main.LocalPlayer.chest != -1) {
			if (confirmationUserInterface?.CurrentState != null) {
				confirmationUserInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);
			}

			if (searchbarUserInterface?.CurrentState != null && !ModContent.GetInstance<BetterChestsConfig>().disableSearchbar) {
				searchbarUserInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);
			}

			if (quickstackLockInterface?.CurrentState != null && Main.LocalPlayer.chest > 0) {
				quickstackLockInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);
			}
		}

		if (chestHoverUserInterface?.CurrentState != null && !ModContent.GetInstance<BetterChestsConfig>().disableChestHover) {
			chestHoverUserInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);
		}

		return true;
	}
	
	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		if (mouseTextIndex == -1) return;

		layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("BetterChests: UI", DrawUI, InterfaceScaleType.UI));
	}

	public static void ToggleSortOptionsUI(SortOptionsUI sortOptionsUI)
	{
		UISystem inst = ModContent.GetInstance<UISystem>();

		SortOptionsUI? currentState = inst.sortOptionsUserInterface?.CurrentState as SortOptionsUI;

		if (currentState?.mode != sortOptionsUI.mode) {
			inst.sortOptionsUserInterface?.SetState(sortOptionsUI);
			return;
		}

		inst.sortOptionsUserInterface?.SetState(null);
	}

	public static void CloseSortOptionsUI()
	{
		ModContent.GetInstance<UISystem>().sortOptionsUserInterface?.SetState(null);
	}

	public static void OpenChestHoverUI(Chest chest)
	{
		UISystem inst = ModContent.GetInstance<UISystem>();

		// don't open the same ChestHoverUI again
		if ((inst.chestHoverUserInterface?.CurrentState as ChestHoverUI)?.chest == chest)
			return;

		// don't open if chest is locked
		if (Chest.IsLocked(chest.x, chest.y)) {
			return;
		}

		inst.chestHoverUserInterface?.SetState(new ChestHoverUI(chest));
	}
	public static void CloseChestHoverUI()
	{
		UISystem inst = ModContent.GetInstance<UISystem>();

		// don't always close the chest
		if (inst.chestHoverUserInterface?.CurrentState == null)
			return;

		inst.chestHoverUserInterface.SetState(null);
	}

	public static void OpenChestUI(int chestID) {
		UISystem inst = ModContent.GetInstance<UISystem>();

		if (!ModContent.GetInstance<BetterChestsConfig>().disableSearchbar) {
			inst.searchbarUserInterface?.SetState(new SearchbarUI());
		}

		if (chestID > 0) {
			inst.quickstackLockInterface?.SetState(new OwnershipLockUI(chestID));
		}
	}

	public static void CloseChestUI()
	{
		UISystem inst = ModContent.GetInstance<UISystem>();
		inst.searchbarUserInterface?.SetState(null);
		inst.quickstackLockInterface?.SetState(null);
		inst.confirmationUserInterface?.SetState(null);
	}

	public static void OpenConfirmationUI(ConfirmationUI ui)
	{
		ModContent.GetInstance<UISystem>().confirmationUserInterface?.SetState(ui);
	}

	public static void CloseConfirmationUI()
	{
		ModContent.GetInstance<UISystem>().confirmationUserInterface?.SetState(null);
	}
}