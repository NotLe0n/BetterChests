using BetterChests.UIElements;
using Terraria;
using Terraria.Localization;
using Terraria.UI;
using System;

namespace BetterChests.UIStates;

internal class SearchbarUI : UIState
{
	private readonly UIBetterTextBox searchBox = new(Language.GetTextValue("Mods.BetterChests.SearchItem")) {
		Top = new(Main.instance.invBottom + 170, 0),
		Left = new(71, 0),
		Width = new(209, 0),
		Height = new(30, 0),
		MaxLength = 20
	};

	public override void OnInitialize()
	{
		base.OnInitialize();

		searchBox.OnTextChanged += () => NewItemSorting.SortChest(x => x.Name.Contains(searchBox.Text, StringComparison.CurrentCultureIgnoreCase), true);
		Append(searchBox);
	}

	public void FocusSearchBox()
	{
		searchBox.Focus();
	}
}
