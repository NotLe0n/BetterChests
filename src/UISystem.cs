using BetterChests.src.UIStates;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests.src;

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

		if (!Main.dedServ)
		{
			SortOptionsUserInterface = new UserInterface();

			ConfirmationUserInterface = new UserInterface();
			ConfirmationUserInterface.SetState(new ConfirmationUI());

			ChestHoverUserInterface = new UserInterface();
			ChestHoverUserInterface.SetState(new ChestHoverUI());

			SearchbarUserInterface = new UserInterface();
			SearchbarUserInterface.SetState(new SearchbarUI());
		}

		base.Load();
	}

	public override void Unload()
	{
		instance = null;

		SortOptionsUserInterface = null;
		ConfirmationUserInterface = null;
		ChestHoverUserInterface = null;
		ChestHoverUI.chest = null;

		base.Unload();
	}

	private GameTime _lastUpdateUiGameTime;
	public override void UpdateUI(GameTime gameTime)
	{
		_lastUpdateUiGameTime = gameTime;

		if (SortOptionsUserInterface.CurrentState != null)
		{
			var currentState = SortOptionsUserInterface.CurrentState as SortOptionsUI;

			if (Main.LocalPlayer.chest != -1 && currentState.mode == SortOptionsMode.Chest)
			{
				SortOptionsUserInterface.Update(gameTime);
			}

			if (Main.playerInventory && currentState.mode == SortOptionsMode.Inventory)
			{
				SortOptionsUserInterface.Update(gameTime);
			}
		}

		if (Main.LocalPlayer.chest != -1)
		{
			if (ConfirmationUI.visible)
			{
				ConfirmationUserInterface.Update(gameTime);
			}

			SearchbarUserInterface.Update(gameTime);

		}
		else
		{
			ConfirmationUI.visible = false;
		}

		if (ChestHoverUI.visible)
		{
			ChestHoverUserInterface.Update(gameTime);
		}
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		if (mouseTextIndex != -1)
		{
			layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
				"BetterChests: UI",
				delegate
				{
					if (SortOptionsUserInterface.CurrentState != null)
					{
						var currentState = SortOptionsUserInterface.CurrentState as SortOptionsUI;

						if (Main.LocalPlayer.chest != -1 && currentState.mode == SortOptionsMode.Chest)
						{
							SortOptionsUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}
						if (Main.playerInventory && currentState.mode == SortOptionsMode.Inventory)
						{
							SortOptionsUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}
					}

					if (Main.LocalPlayer.chest != -1)
					{
						if (ConfirmationUI.visible)
						{
							ConfirmationUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}

						SearchbarUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
					}

					if (ChestHoverUI.visible)
					{
						ChestHoverUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
					}

					return true;
				}, InterfaceScaleType.UI));
		}
	}

	public static void ToggleSortOptionsUI(SortOptionsUI sortOptionsUI)
	{
		SortOptionsUI currentState = instance.SortOptionsUserInterface.CurrentState as SortOptionsUI;

		if (currentState?.mode != sortOptionsUI?.mode)
		{
			instance.SortOptionsUserInterface.SetState(sortOptionsUI);
			return;
		}

		instance.SortOptionsUserInterface.SetState(null);
	}
}
