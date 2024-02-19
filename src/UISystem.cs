using BetterChests.UIStates;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests;

internal class UISystem : ModSystem
{
	public static UISystem instance;

	internal UserInterface SortOptionsUserInterface;
	internal UserInterface ConfirmationUserInterface;
	internal UserInterface ChestHoverUserInterface;
	internal UserInterface SearchbarUserInterface;

	public override void Load()
	{
		instance = this;

		if (!Main.dedServ) {
			SortOptionsUserInterface = new UserInterface();
			SearchbarUserInterface = new UserInterface();
			ConfirmationUserInterface = new UserInterface();
			ChestHoverUserInterface = new UserInterface();
		}

		base.Load();
	}

	public override void Unload()
	{
		instance = null;

		SortOptionsUserInterface = null;
		ConfirmationUserInterface = null;
		ChestHoverUserInterface = null;
		SearchbarUserInterface = null;

		base.Unload();
	}

	private GameTime _lastUpdateUiGameTime;
	public override void UpdateUI(GameTime gameTime)
	{
		_lastUpdateUiGameTime = gameTime;

		if (SortOptionsUserInterface.CurrentState != null) {
			var currentState = SortOptionsUserInterface.CurrentState as SortOptionsUI;

			if (Main.LocalPlayer.chest != -1 && currentState.mode == SortOptionsMode.Chest) {
				SortOptionsUserInterface.Update(gameTime);
			}

			if (Main.playerInventory && currentState.mode == SortOptionsMode.Inventory) {
				SortOptionsUserInterface.Update(gameTime);
			}
		}

		if (Main.LocalPlayer.chest != -1) {
			if (ConfirmationUserInterface.CurrentState != null) {
				ConfirmationUserInterface.Update(gameTime);
			}

			if (SearchbarUserInterface.CurrentState == null)
				SearchbarUserInterface.SetState(new SearchbarUI());

			SearchbarUserInterface.Update(gameTime);

		}
		else {
			ConfirmationUserInterface.SetState(null);
			SearchbarUserInterface.SetState(null);
		}


		if (!Main.tile[Main.MouseWorld.ToTileCoordinates()].HasTile) {
			CloseChestHoverUI();
		}

		if (ChestHoverUserInterface.CurrentState != null && !ModContent.GetInstance<BetterChestsConfig>().disableChestHover) {
			ChestHoverUserInterface.Update(gameTime);
		}
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		if (mouseTextIndex == -1) return;

		layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
			"BetterChests: UI",
			delegate {
				if (SortOptionsUserInterface.CurrentState != null) {
					var currentState = SortOptionsUserInterface.CurrentState as SortOptionsUI;

					if (Main.LocalPlayer.chest != -1 && currentState.mode == SortOptionsMode.Chest) {
						SortOptionsUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
					}
					if (Main.playerInventory && currentState.mode == SortOptionsMode.Inventory) {
						SortOptionsUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
					}
				}

				if (Main.LocalPlayer.chest != -1) {
					if (ConfirmationUserInterface.CurrentState != null)
						ConfirmationUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

					SearchbarUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
				}

				if (ChestHoverUserInterface.CurrentState != null && !ModContent.GetInstance<BetterChestsConfig>().disableChestHover)
					ChestHoverUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

				return true;
			},
			InterfaceScaleType.UI)
		);
	}


	public static void ToggleSortOptionsUI(SortOptionsUI sortOptionsUI)
	{
		SortOptionsUI currentState = instance.SortOptionsUserInterface.CurrentState as SortOptionsUI;

		if (currentState?.mode != sortOptionsUI?.mode) {
			instance.SortOptionsUserInterface.SetState(sortOptionsUI);
			return;
		}

		instance.SortOptionsUserInterface.SetState(null);
	}

	public static void CloseSortOptionsUI()
	{
		instance.SortOptionsUserInterface.SetState(null);
	}

	public static void OpenChestHoverUI(Chest chest)
	{
		// don't open the same ChestHoverUI again
		if ((instance.ChestHoverUserInterface.CurrentState as ChestHoverUI)?.chest == chest)
			return;

		// don't open if chest is locked
		if (Chest.IsLocked(chest.x, chest.y)) {
			return;
		}

		instance.ChestHoverUserInterface.SetState(new ChestHoverUI(chest));
	}
	public static void CloseChestHoverUI()
	{
		// don't always close the chest
		if (instance.ChestHoverUserInterface.CurrentState == null)
			return;

		instance.ChestHoverUserInterface.SetState(null);
	}
}