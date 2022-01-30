using BetterChests.src.UIStates;
using MonoMod.Cil;
using System.Reflection;
using Terraria;
using Terraria.UI;

namespace BetterChests.src;

#pragma warning disable IDE0051 // Remove unused private members

public class ILEdits
{
	public static bool DisableConfirmationButton { get; set; }
	public static string CurrentSortFunction { get; set; }

	public static void Load()
	{
		IL.Terraria.UI.ChestUI.DrawButton += EditButton;
		On.Terraria.UI.ChestUI.Draw += ChestUI_Draw;
		On.Terraria.Player.TileInteractionsMouseOver_Containers += Player_TileInteractionsMouseOver_Containers;
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

	private static void EditButton(ILContext il)
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
	private static void ChestUI_Draw(On.Terraria.UI.ChestUI.orig_Draw orig, Microsoft.Xna.Framework.Graphics.SpriteBatch spritebatch)
	{
		orig(spritebatch);
		// if the "Sort" Button has been clicked once: toggle the Sort option UI
		if (Main.mouseRight && Main.mouseRightRelease && ChestUI.ButtonHovered[ChestUI.ButtonID.Sort])
		{
			// toggle Sort option UI
			SortOptionsUI.visible = !SortOptionsUI.visible;
		}
	}

	private static void CallSortFunction()
	{
		switch (CurrentSortFunction)
		{
			case "Default sort":
				NewItemSorting.DefaultSort(false);
				break;
			case "Sort by ID":
				NewItemSorting.Sort(x => x.type, false);
				break;
			case "Sort alphabetically":
				NewItemSorting.Sort(x => x.Name, false);
				break;
			case "Sort by rarity":
				NewItemSorting.Sort(x => x.rare, true);
				break;
			case "Sort by stack size":
				NewItemSorting.Sort(x => x.stack, true);
				break;
			case "Sort by value":
				NewItemSorting.Sort(x => x.value, true);
				break;
			case "Sort by damage":
				NewItemSorting.Sort(x => x.defense, true);
				break;
			case "Sort by defense":
				NewItemSorting.Sort(x => x.defense, true);
				break;
			case "Sort randomly":
				NewItemSorting.Sort(x => Main.rand.NextFloat(), false);
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
