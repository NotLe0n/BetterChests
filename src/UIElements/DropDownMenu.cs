using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.ModLoader.Config.UI;
using ReLogic.Graphics;
using Newtonsoft.Json;

namespace BetterChests.src.UIElements;

internal class DropDownMenu<T> : ConfigElement
{
	private bool expanded;
	private DropDownItem<T>[] items;
	private T currentSelectedItem;

	public override void OnBind()
	{
		base.OnBind();
		if (MemberInfo.GetValue(Item) is not OptionSelectionPair<T> subitem) {
			var temp = new OptionSelectionPair<T>();
			JsonConvert.PopulateObject(JsonDefaultValueAttribute.Json, temp);
			subitem = temp;

			subitem.selection = subitem.options[0];
		}

		// set current selected item to first item
		currentSelectedItem = subitem.selection;

		// add DropDownItems
		items = new DropDownItem<T>[subitem.options.Length];
		for (int i = subitem.options.Length - 1; i >= 0; i--) {
			items[i] = new DropDownItem<T>(subitem.options[i]);
		}

		// align DropDownItems
		Rectangle dimensions = GetDimensions().ToRectangle();
		float width = LargestItemSize() + 20;
		for (int i = subitem.options.Length - 1; i >= 0; i--) {
			items[i].Left = new(550 - width, 0);
			items[i].Top = new(dimensions.Height + 21 * i + 25, 0);
			items[i].Width = new(width - 10, 0);
			items[i].Height = new(20, 0);
			items[i].OnSelect += (elm) =>
			{
				currentSelectedItem = (elm as DropDownItem<T>).Name;
				subitem.GetType().GetField("selection").SetValue(subitem, currentSelectedItem);
				SetObject(subitem);
				CloseMenu();
			};
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		Rectangle dimensions = GetDimensions().ToRectangle();

		int x = (int)(dimensions.Right - LargestItemSize() - 20);
		int y = dimensions.Y + 5;

		DrawPanel2(spriteBatch, new(x - 2, y - 2), TextureAssets.MagicPixel.Value, LargestItemSize() + 14, 24, Color.Black); // draw black border
		DrawPanel2(spriteBatch, new(x, y), TextureAssets.MagicPixel.Value, LargestItemSize() + 10, 20, new Color(63, 82, 151) * 0.8f);

		spriteBatch.DrawString(FontAssets.MouseText.Value, currentSelectedItem.ToString(), new(x + 5, dimensions.Y + 5), Color.White);
	}

	public override void Click(UIMouseEvent evt)
	{
		ToggleExpand();
	}

	public void ToggleExpand()
	{
		if (expanded) {
			CloseMenu();
		}
		else {
			OpenMenu();
		}
	}

	public void CloseMenu()
	{
		for (int i = 0; i < items.Length; i++) {
			items[i].Remove();
		}
		expanded = false;
	}

	public void OpenMenu()
	{
		for (int i = items.Length - 1; i >= 0; i--) {
			Append(items[i]);
		}
		expanded = true;
	}

	private float LargestItemSize()
	{
		float result = 0;
		foreach (var item in items) {
			float width = FontAssets.MouseText.Value.MeasureString(item.Name.ToString()).X;
			if (width > result) {
				result = width;
			}
		}
		return result;
	}
}
