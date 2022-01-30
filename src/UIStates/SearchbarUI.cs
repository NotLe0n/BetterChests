using BetterChests.src.UIElements;
using Microsoft.Xna.Framework;
using Terraria;
using Microsoft.Xna.Framework.Input;
using Terraria.UI;

namespace BetterChests.src.UIStates
{
	internal class SearchbarUI : UIState
	{
		public override void OnInitialize()
		{
			base.OnInitialize();

			var searchbox = new UIBetterTextBox("search item", Color.White);
			searchbox.Top.Set(Main.instance.invBottom + 170, 0);
			searchbox.Left.Set(71, 0);
			searchbox.Width.Set(209, 0);
			searchbox.Height.Set(30, 0);
			searchbox.OnTextChanged += () =>
			{
				NewItemSorting.Sort(x => x.Name.ToLower().Contains(searchbox.currentString.ToLower()), true);
			};
			searchbox.OnKeyPressed += (key) =>
			{
				if (key == Keys.Escape)
				{
					searchbox.Unfocus();
				}
			};
			Append(searchbox);
		}
	}
}
