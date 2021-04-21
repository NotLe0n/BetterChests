using BetterChests.src.UIElements;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests.src.UIStates
{
    internal class SortOptionsUI : UIState
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
            list.Width.Set(200, 0);
            list.Height.Set(400, 0);
            list.ListPadding = 14;
            Append(list);

            AddSortOption("Default sort", (evt, elm) => ItemSorting.SortChest());
            AddSortOption("Sort by ID", (evt, elm) => Sort(x => x.type, _reversed));
            AddSortOption("Sort Alphabetically", (evt, elm) => Sort(x => x.Name, _reversed));
            AddSortOption("Sort by rarity", (evt, elm) => Sort(x => x.rare, !_reversed));
            AddSortOption("Sort by stack size", (evt, elm) => Sort(x => x.stack, !_reversed));
            AddSortOption("Sort by value", (evt, elm) => Sort(x => x.value, !_reversed));
            AddSortOption("Sort by damage", (evt, elm) => Sort(x => x.damage, !_reversed));
            AddSortOption("Sort by defense", (evt, elm) => Sort(x => x.defense, !_reversed));
            AddSortOption("Sort randomly", (evt, elm) => Sort(x => Main.rand.NextFloat(), _reversed));

            var option = new UITextOption("Reversed: No");
            option.MarginTop = 2f;
            option.MarginLeft = 2f;
            option.OnClick += (evt, elm) => 
            { 
                _reversed = !_reversed; 
                option.SetText(_reversed ? "Reversed: Yes" : "Reversed: No");
            };
            list.Add(option);

            var searchbox = new UIBetterTextBox("search item", Color.White);
            searchbox.Top.Set(Main.instance.invBottom + 170, 0);
            searchbox.Left.Set(71, 0);
            searchbox.Width.Set(209, 0);
            searchbox.Height.Set(30, 0);
            searchbox.OnTextChanged += () => Sort(x => x.Name.ToLower().Contains(searchbox.currentString.ToLower()), true);
            Append(searchbox);
        }

        private void AddSortOption(string title, MouseEvent onclick)
        {
            var option = new UITextOption(title);
            option.MarginTop = 2f;
            option.MarginLeft = 2f;
            option.OnClick += onclick;
            list.Add(option);
        }

        private void Sort<T>(Func<Item, T> func, bool reversed)
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