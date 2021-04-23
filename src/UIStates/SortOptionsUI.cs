using BetterChests.src.UIElements;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
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
            header.Left.Set(506 + 130, 0); // magic number because vanilla does it the same way lmao
            Append(header);

            list = new UIList();
            list.Top.Set(Main.instance.invBottom + 30, 0);
            list.Left.Set(506 + 130, 0);
            list.Width.Set(400, 0);
            list.Height.Set(400, 0);
            list.ListPadding = 14;
            list.SetPadding(2);
            Append(list);

            AddSortOption("Default sort", (evt, elm) => ItemSorting.SortChest());
            AddSortOption("Sort by ID", (evt, elm) => NewItemSorting.Sort(x => x.type, _reversed));
            AddSortOption("Sort Alphabetically", (evt, elm) => NewItemSorting.Sort(x => x.Name, _reversed));
            AddSortOption("Sort by rarity", (evt, elm) => NewItemSorting.Sort(x => x.rare, !_reversed));
            AddSortOption("Sort by stack size", (evt, elm) => NewItemSorting.Sort(x => x.stack, !_reversed));
            AddSortOption("Sort by value", (evt, elm) => NewItemSorting.Sort(x => x.value, !_reversed));
            AddSortOption("Sort by damage", (evt, elm) => NewItemSorting.Sort(x => x.damage, !_reversed));
            AddSortOption("Sort by defense", (evt, elm) => NewItemSorting.Sort(x => x.defense, !_reversed));
            AddSortOption("Sort by Mod", new MouseEvent(ModCarusel));
            AddSortOption("Sort randomly", (evt, elm) => NewItemSorting.Sort(x => Main.rand.NextFloat(), _reversed));

            var option = new UITextOption("Reversed: No");
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
            searchbox.OnTextChanged += () => NewItemSorting.Sort(x => x.Name.ToLower().Contains(searchbox.currentString.ToLower()), true);
            Append(searchbox);
        }

        private void AddSortOption(string title, MouseEvent onclick)
        {
            var option = new UITextOption(title);
            option.OnClick += onclick;
            list.Add(option);
        }

        private int caruselIndex = 0;
        private void ModCarusel(UIMouseEvent evt, UIElement elm)
        {
            // get list of all mods with items
            List<Mod> modsWithItems = new List<Mod>();
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = ItemLoader.GetItem(i);
                if (item != null && !modsWithItems.Contains(item.mod))
                {
                    modsWithItems.Add(item.mod);
                }
            }

            // increase index and reset it if limit is reached
            caruselIndex = caruselIndex < modsWithItems.Count - 1 ? caruselIndex + 1 : 0;

            // set text to mod name
            (elm as UITextOption).SetText("Sort my Mod: " + modsWithItems[caruselIndex].Name);
            // sort items
            NewItemSorting.Sort(x => x.modItem != null && x.modItem.mod.Name == modsWithItems[caruselIndex].Name, !_reversed);
        }
    }
}