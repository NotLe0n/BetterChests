using BetterChests.UIElements;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests.UIStates;

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

		var header = new UIText(Language.GetTextValue("Mods.BetterChests.SortingOptions"));
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

		AddSortOption(SortOptions.Default, _ => NewItemSorting.SortByMode(_reversed, mode));
		AddSortOption(SortOptions.ID, _ => NewItemSorting.SortByMode(i => i.type, _reversed, mode));
		AddSortOption(SortOptions.Alphabetically, _ => NewItemSorting.SortByMode(i => i.Name, _reversed, mode));
		AddSortOption(SortOptions.Rarity, _ => NewItemSorting.SortByMode(i => i.rare, !_reversed, mode));
		AddSortOption(SortOptions.Stack, _ => NewItemSorting.SortByMode(i => i.stack, !_reversed, mode));
		AddSortOption(SortOptions.Value, _ => NewItemSorting.SortByMode(i => i.value, !_reversed, mode));
		AddSortOption(SortOptions.Damage, _ => NewItemSorting.SortByMode(i => i.damage, !_reversed, mode));
		AddSortOption(SortOptions.Defense, _ => NewItemSorting.SortByMode(i => i.defense, !_reversed, mode));
		AddSortOption(SortOptions.Mod, ModCarusel);
		AddSortOption(SortOptions.Random, _ => NewItemSorting.SortByMode(_ => Main.rand.NextFloat(), _reversed, mode));
		
		var option = new UITextOption($"{Language.GetTextValue("Mods.BetterChests.Reversed")}: {Language.GetTextValue("CLI.No")}");
		option.OnLeftClick += (_, _) =>
		{
			_reversed = !_reversed;
			option.SetText($"{Language.GetTextValue("Mods.BetterChests.Reversed")}: {Language.GetTextValue("CLI." + (_reversed ? "Yes" : "No"))}");
		};
		list.Add(option);
	}

	private void AddSortOption(SortOptions sortOption, Action<UITextOption> onclick)
	{
		var option = new UITextOption(((SortOption)sortOption).ToString());
		option.OnLeftClick += (_, _) => onclick(option);
		list.Add(option);
	}

	private int caruselIndex;
	private void ModCarusel(UITextOption elm)
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
		elm.SetText($"{Language.GetTextValue("Mods.BetterChests.SortOptions.Mod.Label")}: {modsWithItems[caruselIndex].Name}");

		// sort items
		NewItemSorting.SortByMode(x => x.ModItem != null && x.ModItem.Mod.Name == modsWithItems[caruselIndex].Name, !_reversed, mode);
	}
}
