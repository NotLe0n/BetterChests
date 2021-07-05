using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.UI;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BetterChests.src
{
    class Hotkeys : ModPlayer
    {
        public static ModHotKey DepositAll, LootAll, QuickStack, Restock, SortChest, SortInventory;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            base.ProcessTriggers(triggersSet);

            if (DepositAll.JustPressed)
            {
                ChestUI.DepositAll();
            }
            if (LootAll.JustPressed)
            {
                ChestUI.LootAll();
            }
            if (QuickStack.JustPressed)
            {
                player.QuickStackAllChests();
                ChestUI.QuickStack();
            }
            if (Restock.JustPressed)
            {
                ChestUI.Restock();
            }
            if (SortChest.JustPressed)
            {
                ItemSorting.SortChest();
            }
            if (SortInventory.JustPressed)
            {
                ItemSorting.SortInventory();
            }
        }
    }
}
