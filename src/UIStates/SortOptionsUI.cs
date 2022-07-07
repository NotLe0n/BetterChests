using BetterChests.src.UIElements;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests.src.UIStates;

internal class SortOptionsUI : UIState
{
	public SortOptionsMode mode;
	private bool _reversed;
	private readonly UIList list;

	public SortOptionsUI(SortOptionsMode mode)
	{
		this.mode = mode;
		_reversed = false;

		int x = mode == SortOptionsMode.Chest ? 506 + 130 : 534 + 50;
		int y = mode == SortOptionsMode.Chest ? Main.instance.invBottom : 244;

		var header = new UIText("Sorting Options");
		header.Top.Set(y, 0);
		header.Left.Set(x, 0); // magic number because vanilla does it the same way lmao
		Append(header);

		list = new UIList();
		list.Top.Set(y + 30, 0);
		list.Left.Set(x, 0);
		list.Width.Set(400, 0);
		list.Height.Set(400, 0);
		list.ListPadding = 14;
		list.SetPadding(2);
		Append(list);

		AddSortOption("Default sort", (evt, elm) => NewItemSorting.SortByMode(_reversed, mode));
		AddSortOption("Sort by ID", (evt, elm) => NewItemSorting.SortByMode(x => x.type, _reversed, mode));
		AddSortOption("Sort alphabetically", (evt, elm) => NewItemSorting.SortByMode(x => x.Name, _reversed, mode));
		AddSortOption("Sort by rarity", (evt, elm) => NewItemSorting.SortByMode(x => x.rare, !_reversed, mode));
		AddSortOption("Sort by stack size", (evt, elm) => NewItemSorting.SortByMode(x => x.stack, !_reversed, mode));
		AddSortOption("Sort by value", (evt, elm) => NewItemSorting.SortByMode(x => x.value, !_reversed, mode));
		AddSortOption("Sort by damage", (evt, elm) => NewItemSorting.SortByMode(x => x.damage, !_reversed, mode));
		AddSortOption("Sort by defense", (evt, elm) => NewItemSorting.SortByMode(x => x.defense, !_reversed, mode));
		AddSortOption("Sort by Mod", new MouseEvent(ModCarusel));
		AddSortOption("Sort randomly", (evt, elm) => NewItemSorting.SortByMode(x => Main.rand.NextFloat(), _reversed, mode));

		var option = new UITextOption("Reversed: No");
		option.OnClick += (evt, elm) =>
		{
			_reversed = !_reversed;
			option.SetText(_reversed ? "Reversed: Yes" : "Reversed: No");
		};
		list.Add(option);
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
		var modsWithItems = new List<Mod>();
		for (int i = 0; i < ItemLoader.ItemCount; i++) {
			var item = ItemLoader.GetItem(i);
			if (item != null && !modsWithItems.Contains(item.Mod)) {
				modsWithItems.Add(item.Mod);
			}
		}

		// increase index and reset it if limit is reached
		caruselIndex = ++caruselIndex % modsWithItems.Count;

		// set text to mod name
		(elm as UITextOption).SetText("Sort my Mod: " + modsWithItems[caruselIndex].Name);

		// sort items
		NewItemSorting.SortByMode(x => x.ModItem != null && x.ModItem.Mod.Name == modsWithItems[caruselIndex].Name, !_reversed, mode);
	}
}
