using BetterChests.src.UIElements;
using Terraria;
using Terraria.UI;

namespace BetterChests.src.UIStates;

internal class SearchbarUI : UIState
{
	public override void OnInitialize()
	{
		base.OnInitialize();

		var searchbox = new UIBetterTextBox("search item")
		{
			Top = new(Main.instance.invBottom + 170, 0),
			Left = new(71, 0),
			Width = new(209, 0),
			Height = new(30, 0),
			MaxLength = 20
		};
		searchbox.OnTextChanged += () => NewItemSorting.SortChest(x => x.Name.ToLower().Contains(searchbox.Text.ToLower()), true);
		Append(searchbox);
	}
}
