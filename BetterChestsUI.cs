using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria;
using System.Linq;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BetterChests
{
    class BetterChestsUI : UIState
    {
        public static bool visible = false;
        private bool _reversed = false;
        private UIList list;

        public override void OnInitialize()
        {
            var header = new UIText("Sorting Options");
            header.Top.Set(Main.instance.invBottom, 0);
            header.Left.Set(506 + 100, 0); // magic number because vanilla does it the same way lmao
            Append(header);

            list = new UIList();
            list.Top.Set(Main.instance.invBottom + 30, 0);
            list.Left.Set(506 + 100, 0);
            list.Width.Set(300, 0);
            list.Height.Set(400, 0);
            list.ListPadding = 14;
            Append(list);

            AddSortOption("Default sort", (evt, elm) => ItemSorting.SortChest());
            AddSortOption("Sort by ID", (evt, elm) => Sort(x => x.netID, _reversed));
            AddSortOption("Sort by name", (evt, elm) => Sort(x => x.Name, _reversed));
            AddSortOption("Sort by rarity", (evt, elm) => Sort(x => x.rare, !_reversed));
            AddSortOption("Sort by stack size", (evt, elm) => Sort(x => x.stack, !_reversed));
            AddSortOption("Sort by value", (evt, elm) => Sort(x => x.value, !_reversed));
            AddSortOption("Sort by damage", (evt, elm) => Sort(x => x.damage, !_reversed));
            AddSortOption("Sort by defense", (evt, elm) => Sort(x => x.defense, !_reversed));

        }

        private void AddSortOption(string title, MouseEvent onclick)
        {
            var option = new UITextOption(title);
            option.OnClick += onclick;
            list.Add(option);
        }

        private void Sort<T>(Func<Item, T> func, bool reversed)
        {
            // all items in the chest
            ref var items = ref Main.chest[Main.LocalPlayer.chest].item;

            // order the items according to the function.
            var sortedItems = items.OrderBy(func).ToArray();

            if (reversed)
            {
                // reverse the order
                sortedItems = sortedItems.Reverse().ToArray();
            }

            // Air always goes last
            sortedItems = sortedItems.OrderBy(x => x.IsAir).ToArray();

            // Change color of changed slots
            for (int i = 0; i < items.Length; i++)
            {
                if (!items[i].IsAir && items[i] != sortedItems[i])
                {
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}