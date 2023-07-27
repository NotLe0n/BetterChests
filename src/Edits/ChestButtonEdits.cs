using BetterChests.UIStates;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.UI;

namespace BetterChests.Edits;

// ReSharper disable UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members

internal class ChestButtonEdits
{
	public static bool DisableConfirmationButton { get; set; }
	public static SortOptions CurrentSortFunction { get; set; }

	public static void Load()
	{
		IL_ChestUI.DrawButton += EditChestButtons;
		On_Main.DrawInventory += OpenSortInventoryOptionsLogic;
		On_ChestUI.Draw += OpenSortChestOptionsLogic;
		On_Player.OpenChest += ResetAlreadyClicked;
	}

	private static void OpenSortInventoryOptionsLogic(On_Main.orig_DrawInventory orig, Main self)
	{
		orig(self);
		const int buttonX = 534;
		const int buttonY = 244;
		const int buttonWidth = 30;
		const int buttonHeight = 30;

		// true if the mouse is hovering over the sort inventory button
		if (Main.mouseX >= buttonX && Main.mouseX <= buttonX + buttonWidth && Main.mouseY >= buttonY && Main.mouseY <= buttonY + buttonHeight && !PlayerInput.IgnoreMouseInterface) {
			// true if the user right clicked
			if (Main.mouseRight && Main.mouseRightRelease) {
				Main.mouseRightRelease = false;

				// toggle Sort option UI
				UISystem.ToggleSortOptionsUI(new SortOptionsUI(SortOptionsMode.Inventory));
			}
		}
	}

	private static void EditChestButtons(ILContext il)
	{
		BindingFlags nonPubS = BindingFlags.NonPublic | BindingFlags.Static;
		var c = new ILCursor(il);

		// IL_0362: br.s      IL_038C
		// IL_0364: call      void Terraria.UI.ChestUI::LootAll()
		//      <=== here

		if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ChestUI>(nameof(ChestUI.LootAll))))
			throw new($"{nameof(EditChestButtons)} IL Edit failed at {nameof(ChestUI.LootAll)}");
		
		// call own method instead of LootAll()
		c.Prev.Operand = typeof(ChestButtonEdits).GetMethod(nameof(OpenLootConfirmation), nonPubS);

		// IL_0362: br.s      IL_038C
		// IL_0364: call      void Terraria.UI.ChestUI::DepositAll(ContainerTransferContext)
		//      <=== here

		if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ChestUI>(nameof(ChestUI.DepositAll))))
			throw new($"{nameof(EditChestButtons)} IL Edit failed at {nameof(ChestUI.DepositAll)}");

		// call own method instead of DepositAll(ContainerTransferContext)
		c.Prev.Operand = typeof(ChestButtonEdits).GetMethod(nameof(OpenDepositConfirmation), nonPubS);

		// The goal of this IL edit is to replace SortChest() with code to open my UI
		// IL_0385: br.s      IL_038C
		// IL_0387: call      void Terraria.UI.ItemSorting::SortChest()
		//      <=== here

		if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ItemSorting>(nameof(ItemSorting.SortChest))))
			throw new($"{nameof(EditChestButtons)} IL Edit failed at {nameof(ItemSorting.SortChest)}");

		// call own method instead of SortChest()
		c.Prev.Operand = typeof(ChestButtonEdits).GetMethod(nameof(CallSortFunction), nonPubS);
	}

	// Draw method because that calls every tick
	private static void OpenSortChestOptionsLogic(On_ChestUI.orig_Draw orig, Microsoft.Xna.Framework.Graphics.SpriteBatch spritebatch)
	{
		orig(spritebatch);
		// if the "Sort" Button has been clicked once: toggle the Sort option UI
		if (Main.mouseRight && Main.mouseRightRelease && ChestUI.ButtonHovered[ChestUI.ButtonID.Sort]) {
			Main.mouseRightRelease = false;

			// toggle Sort option UI
			UISystem.ToggleSortOptionsUI(new SortOptionsUI(SortOptionsMode.Chest));
		}
	}

	private static void CallSortFunction()
	{
		switch (CurrentSortFunction) {
			case SortOptions.Default:
				NewItemSorting.DefaultChestSort(false);
				break;
			case SortOptions.ID:
				NewItemSorting.SortChest(x => x.type, false);
				break;
			case SortOptions.Alphabetically:
				NewItemSorting.SortChest(x => x.Name, false);
				break;
			case SortOptions.Rarity:
				NewItemSorting.SortChest(x => x.rare, true);
				break;
			case SortOptions.Stack:
				NewItemSorting.SortChest(x => x.stack, true);
				break;
			case SortOptions.Value:
				NewItemSorting.SortChest(x => x.value, true);
				break;
			case SortOptions.Damage:
				NewItemSorting.SortChest(x => x.damage, true);
				break;
			case SortOptions.Defense:
				NewItemSorting.SortChest(x => x.defense, true);
				break;
			case SortOptions.Random:
				NewItemSorting.SortChest(_ => Main.rand.NextFloat(), false);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	// to prevent repeatidly clicking the button when held
	private static bool alreadyClicked;

	// this is called when the player clicks on "Deposit All" in the Chest UI
	private static void OpenDepositConfirmation(ContainerTransferContext context)
	{
		// if confirmations are disabled call DepositAll normally
		if (DisableConfirmationButton) {
			ChestUI.DepositAll(context);
			return;
		}

		// if the button was already clicked last call: skip
		if (alreadyClicked) return;

		alreadyClicked = true;
		var ui = new ConfirmationUI(ChestUI.ButtonID.DepositAll, 55, () =>
		{
			ChestUI.DepositAll(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
			UISystem.instance.ConfirmationUserInterface.SetState(null);
			alreadyClicked = false;
		});

		UISystem.instance.ConfirmationUserInterface.SetState(ui);
	}

	// this is called when the player clicks on "Loot All" in the Chest UI
	private static void OpenLootConfirmation()
	{
		// if confirmations are disabled call LootAll normally
		if (DisableConfirmationButton) {
			ChestUI.LootAll();
			return;
		}

		// if the button was already clicked last call: skip
		if (alreadyClicked) return;

		alreadyClicked = true;
		var ui = new ConfirmationUI(ChestUI.ButtonID.LootAll, 30, () =>
		{
			ChestUI.LootAll();
			UISystem.instance.ConfirmationUserInterface.SetState(null);
			alreadyClicked = false;
		});

		UISystem.instance.ConfirmationUserInterface.SetState(ui);
	}

	// reset already clicked when opening a chest
	private static void ResetAlreadyClicked(On_Player.orig_OpenChest orig, Player self, int x, int y, int newChest)
	{
		orig(self, x, y, newChest);
		alreadyClicked = false;
	}
}
