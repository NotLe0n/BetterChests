using BetterChests.src.UIStates;
using MonoMod.Cil;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace BetterChests.src;

#pragma warning disable IDE0051 // Remove unused private members

public class ILEdits
{
	public static bool DisableConfirmationButton { get; set; }
	public static string CurrentSortFunction { get; set; }

	public static void Load()
	{
		IL.Terraria.UI.ChestUI.DrawButton += EditChestButtons;
		On.Terraria.Main.DrawInventory += OpenSortInventoryOptionsLogic;
		On.Terraria.UI.ChestUI.Draw += OpenSortChestOptionsLogic;
		On.Terraria.Player.TileInteractionsMouseOver_Containers += Player_TileInteractionsMouseOver_Containers;
	}

	private static void OpenSortInventoryOptionsLogic(On.Terraria.Main.orig_DrawInventory orig, Main self)
	{
		orig(self);
		const int buttonX = 534;
		const int buttonY = 244;
		const int buttonWidth = 30;
		const int buttonHeight = 30;

		// true if the mouse is hovering over the sort inventory button
		if (Main.mouseX >= buttonX && Main.mouseX <= buttonX + buttonWidth && Main.mouseY >= buttonY && Main.mouseY <= buttonY + buttonHeight && !PlayerInput.IgnoreMouseInterface)
		{
			// true if the user right clicked
			if (Main.mouseRight && Main.mouseRightRelease)
			{
				Main.mouseRightRelease = false;

				// toggle Sort option UI
				UISystem.ToggleSortOptionsUI(new SortOptionsUI(SortOptionsMode.Inventory));
			}
		}
	}

	// TODO: Find method which allows piggy bank to also be hoverable
	private static void Player_TileInteractionsMouseOver_Containers(On.Terraria.Player.orig_TileInteractionsMouseOver_Containers orig, Player self, int myX, int myY)
	{
		orig(self, myX, myY);

		Tile tile = Main.tile[myX, myY];

		int chestX = myX;
		int chestY = myY;
		if (tile.frameX % 36 != 0)
		{
			chestX--;
		}
		if (tile.frameY % 36 != 0)
		{
			chestY--;
		}

		ChestHoverUI.chest = Main.chest[Chest.FindChest(chestX, chestY)];
		ChestHoverUI.visible = true;
		alreadyClicked = false;
	}

	private static void EditChestButtons(ILContext il)
	{
		var c = new ILCursor(il);

		if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ChestUI>("LootAll")))
			return;

		// call own method instead of LootAll()
		c.Prev.Operand = typeof(ILEdits).GetMethod("OpenLootConfirmation", BindingFlags.NonPublic | BindingFlags.Static);

		// IL_0362: br.s      IL_038C
		// IL_0364: call      void Terraria.UI.ChestUI::DepositAll()
		//      <=== here

		if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ChestUI>("DepositAll")))
			return;

		// call own method instead of DepositAll()
		c.Prev.Operand = typeof(ILEdits).GetMethod("OpenDepositConfirmation", BindingFlags.NonPublic | BindingFlags.Static);

		// The goal of this IL edit is to replace SortChest() with code to open my UI
		// IL_0385: br.s      IL_038C
		// IL_0387: call      void Terraria.UI.ItemSorting::SortChest()
		//      <=== here

		if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ItemSorting>("SortChest")))
			return;

		// call own method instead of SortChest()
		c.Prev.Operand = typeof(ILEdits).GetMethod("CallSortFunction", BindingFlags.NonPublic | BindingFlags.Static);
	}

	// Draw method because that calls every tick
	private static void OpenSortChestOptionsLogic(On.Terraria.UI.ChestUI.orig_Draw orig, Microsoft.Xna.Framework.Graphics.SpriteBatch spritebatch)
	{
		orig(spritebatch);
		// if the "Sort" Button has been clicked once: toggle the Sort option UI
		if (Main.mouseRight && Main.mouseRightRelease && ChestUI.ButtonHovered[ChestUI.ButtonID.Sort])
		{
			Main.mouseRightRelease = false;

			// toggle Sort option UI
			UISystem.ToggleSortOptionsUI(new SortOptionsUI(SortOptionsMode.Chest));
		}
	}

	private static void CallSortFunction()
	{
		switch (CurrentSortFunction)
		{
			case "Default sort":
				NewItemSorting.DefaultChestSort(false);
				break;
			case "Sort by ID":
				NewItemSorting.SortChest(x => x.type, false);
				break;
			case "Sort alphabetically":
				NewItemSorting.SortChest(x => x.Name, false);
				break;
			case "Sort by rarity":
				NewItemSorting.SortChest(x => x.rare, true);
				break;
			case "Sort by stack size":
				NewItemSorting.SortChest(x => x.stack, true);
				break;
			case "Sort by value":
				NewItemSorting.SortChest(x => x.value, true);
				break;
			case "Sort by damage":
				NewItemSorting.SortChest(x => x.defense, true);
				break;
			case "Sort by defense":
				NewItemSorting.SortChest(x => x.defense, true);
				break;
			case "Sort randomly":
				NewItemSorting.SortChest(x => Main.rand.NextFloat(), false);
				break;
			default:
				break;
		}
	}

	// to prevent repeatidly clicking the button when held
	private static bool alreadyClicked;

	// this is called when the player clicks on "Deposit All" in the Chest UI
	private static void OpenDepositConfirmation()
	{
		// if confirmations are disabled call DepositAll normally
		if (DisableConfirmationButton)
		{
			ChestUI.DepositAll();
			return;
		}

		// if the button was already clicked last call: skip
		if (alreadyClicked) return;

		alreadyClicked = true;
		var ui = new ConfirmationUI
		{
			buttonID = ChestUI.ButtonID.DepositAll,
			topOffset = 55,
			onclick = (evt, elm) =>
			{
				ChestUI.DepositAll();
				ConfirmationUI.visible = false;
				alreadyClicked = false;
			}
		};

		UISystem.instance.ConfirmationUserInterface.SetState(ui);
		ConfirmationUI.visible = true;
	}

	// this is called when the player clicks on "Loot All" in the Chest UI
	private static void OpenLootConfirmation()
	{
		// if confirmations are disabled call LootAll normally
		if (DisableConfirmationButton)
		{
			ChestUI.LootAll();
			return;
		}

		// if the button was already clicked last call: skip
		if (alreadyClicked) return;

		alreadyClicked = true;
		var ui = new ConfirmationUI
		{
			buttonID = ChestUI.ButtonID.LootAll,
			topOffset = 30,
			onclick = (evt, elm) =>
			{
				ChestUI.LootAll();
				ConfirmationUI.visible = false;
				alreadyClicked = false;
			}
		};

		UISystem.instance.ConfirmationUserInterface.SetState(ui);
		ConfirmationUI.visible = true;
	}
}
