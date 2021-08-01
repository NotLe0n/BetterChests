using BetterChests.src.UIStates;
using MonoMod.Cil;
using System.Reflection;
using Terraria;
using Terraria.UI;

namespace BetterChests.src
{
    public class ILEdits
    {
        public static void Load()
        {
            IL.Terraria.UI.ChestUI.DrawButton += EditButton;
            On.Terraria.Player.TileInteractionsMouseOver_Containers += Player_TileInteractionsMouseOver_Containers;
        }

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
            Chest chest = Main.chest[Chest.FindChest(chestX, chestY)];
            ChestHoverUI.chest = chest;
            ChestHoverUI.visible = true;
            clicked = false;
        }

        private static void EditButton(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ChestUI>("LootAll")))
                return;

            c.Prev.Operand = typeof(ILEdits).GetMethod("OpenLootConfirmation", BindingFlags.NonPublic | BindingFlags.Static);

            // IL_0362: br.s      IL_038C
            // IL_0364: call      void Terraria.UI.ChestUI::DepositAll()
            //      <=== here

            if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ChestUI>("DepositAll")))
                return;

            c.Prev.Operand = typeof(ILEdits).GetMethod("OpenDepositConfirmation", BindingFlags.NonPublic | BindingFlags.Static);

            // The goal of this IL edit is to replace SortChest() with code to open my UI
            // IL_0385: br.s      IL_038C
            // IL_0387: call      void Terraria.UI.ItemSorting::SortChest()
            //      <=== here

            if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ItemSorting>("SortChest")))
                return;

            c.Prev.Operand = typeof(ILEdits).GetMethod("ToggleSortUI", BindingFlags.NonPublic | BindingFlags.Static);
        }

        private static void ToggleSortUI()
        {
            SortOptionsUI.visible = !SortOptionsUI.visible;
        }

        private static bool clicked = false;
        private static void OpenDepositConfirmation()
        {
            if (clicked) return;

            clicked = true;
            var ui = new ConfirmationUI
            {
                buttonID = ChestUI.ButtonID.DepositAll,
                topOffset = 55,
                onclick = (evt, elm) =>
                {
                    ChestUI.DepositAll();
                    ConfirmationUI.visible = false;
                    clicked = false;
                }
            };

            UISystem.instance.ConfirmationUserInterface.SetState(ui);
            ConfirmationUI.visible = true;
        }

        private static void OpenLootConfirmation()
        {
            if (clicked) return;

            clicked = true;
            var ui = new ConfirmationUI
            {
                buttonID = ChestUI.ButtonID.LootAll,
                topOffset = 30,
                onclick = (evt, elm) =>
                {
                    ChestUI.LootAll();
                    ConfirmationUI.visible = false;
                    clicked = false;
                }
            };

            UISystem.instance.ConfirmationUserInterface.SetState(ui);
            ConfirmationUI.visible = true;
        }
    }
}
