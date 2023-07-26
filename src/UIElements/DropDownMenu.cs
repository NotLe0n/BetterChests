using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ReLogic.Graphics;
using System.Linq;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace BetterChests.UIElements;

internal class DropDownMenu<T> : ConfigElement
{
	private bool expanded;
	private bool changed;
	private DropDownItem<T>[] items;
	private T currentSelectedItem;

	public override void OnBind()
	{
		base.OnBind();
		var subItem = (OptionSelectionPair<T>)MemberInfo.GetValue(Item);
		if (subItem.selection is null || subItem.options is null) {
			subItem = JsonConvert.DeserializeObject<OptionSelectionPair<T>>(JsonDefaultValueAttribute.Json);
		}

		// set current selected item to first item
		currentSelectedItem = subItem.selection;

		// add DropDownItems
		items = new DropDownItem<T>[subItem.options.Length];
		Rectangle dimensions = GetDimensions().ToRectangle();
		for (int i = 0; i < subItem.options.Length; i++) {
			items[i] = new DropDownItem<T>(subItem.options[i], elm =>
			{
				currentSelectedItem = ((DropDownItem<T>)elm).Name;
				subItem.selection = currentSelectedItem;
				UpdateElement(subItem);
			}) {
				Height = new(20, 0),
				Top = new(dimensions.Height + 19 * i + 25, 0)
			};
		}

		float width = LargestItemSize() + 20;
		foreach (DropDownItem<T> item in items) {
			item.Width.Pixels = width - 10;
			item.Left.Pixels = 550 - width;
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		Rectangle dimensions = GetDimensions().ToRectangle();

		int x = (int)(dimensions.Right - LargestItemSize() - 20);
		int y = dimensions.Y + 5;

		DrawPanel2(spriteBatch, new(x - 2, y - 2), TextureAssets.MagicPixel.Value, LargestItemSize() + 14, 24, Color.Black * 0.8f); // draw black border
		DrawPanel2(spriteBatch, new(x, y), TextureAssets.MagicPixel.Value, LargestItemSize() + 10, 20, new Color(63, 82, 151) * 0.8f);

		spriteBatch.DrawString(FontAssets.MouseText.Value, currentSelectedItem.ToString(), new(x + 5, dimensions.Y + 5), Color.White);
	}

	public override void Update(GameTime gameTime)
	{
		if (changed) {
			CloseMenu();
			changed = false;
		}
		
		base.Update(gameTime);
	}

	public override void LeftClick(UIMouseEvent evt)
	{
		ToggleExpand();
	}

	private void UpdateElement(OptionSelectionPair<T> update)
	{
		SetObject(update);
		changed = true;
	}

	private void ToggleExpand()
	{
		if (expanded) {
			CloseMenu();
		}
		else {
			OpenMenu();
		}
	}

	private void CloseMenu()
	{
		foreach (var item in items) {
			item.Remove();
		}
		expanded = false;
	}

	private void OpenMenu()
	{
		foreach (var item in items) {
			Append(item);
		}
		expanded = true;
	}
	
	private float LargestItemSize()
	{
		return items.Max(item => FontAssets.MouseText.Value.MeasureString(item.Name.ToString()).X);
	}
}
