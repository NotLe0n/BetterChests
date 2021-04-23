using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests.src
{
    public class NewItemSorting
    {
        public static void Sort<T>(Func<Item, T> func, bool reversed)
        {
            // all items in the chest
            ref var items = ref Main.chest[Main.LocalPlayer.chest].item;

            // order the items according to the function.
            var sortedItems = items.OrderBy(func).ThenBy(x => x.type).ToArray();

            if (reversed)
            {
                // reverse the order
                sortedItems = sortedItems.Reverse().ToArray();
            }

            // Air always goes last
            sortedItems = sortedItems.OrderBy(x => x.IsAir).ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                if (!sortedItems[i].IsAir && items[i] != sortedItems[i])
                {
                    // Change color of changed slots
                    ItemSlot.SetGlow(i, Main.rand.NextFloat(), true);
                }
            }

            // Apply changes
            items = sortedItems;

            // sync chest contents with all clients
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    NetMessage.SendData(MessageID.SyncChestItem, number: Main.LocalPlayer.chest, number2: i);
                }
            }
        }
    }
}
