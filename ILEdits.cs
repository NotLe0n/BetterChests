using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using System.Reflection;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Terraria.UI;
using System.Diagnostics;

namespace BetterChests
{
    public class ILEdits
    {
        public static void Load()
        {
            IL.Terraria.UI.ChestUI.DrawButton += EditButton;
        }

        private static void EditButton(ILContext il)
        {
            var c = new ILCursor(il);

            // The goal of this IL edit is to replace SortChest() with code to open my UI
            // IL_0385: br.s      IL_038C
            // IL_0387: call      void Terraria.UI.ItemSorting::SortChest()
            //      <=== here

            if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<ItemSorting>("SortChest")))
                return;

            c.Prev.Operand = typeof(ILEdits).GetMethod("OpenUI", BindingFlags.NonPublic | BindingFlags.Static);
        }

        private static void OpenUI()
        {
            BetterChestsUI.visible = !BetterChestsUI.visible;
        }
    }
}
