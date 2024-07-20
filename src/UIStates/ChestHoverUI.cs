using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests.UIStates;

internal class ChestHoverUI(Chest chest) : UIState
{
	public readonly Chest chest = chest;

    public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);

		if (chest == null)
			return;

		// all items in the chest
		Item[] items = chest.item.Where(x => x != null && x.type != ItemID.None).ToArray();

		const int YOffset = 40;
		const int Padding = 20;
		const int MaxSize = 20;

		int collumn = 0;
		int row = 0;
		for (int i = 0; i < items.Length; i++) {
			// set positions (10 items per row)
			if (i % 10 == 0) {
				row += MaxSize + Padding;
				collumn = 0;
			}

			Main.instance.LoadItem(items[i].type); // load item before trying to get its texture (Item only gets loaded once)
			Texture2D itemTexture = TextureAssets.Item[items[i].type].Value; // get item texture
			Vector2 drawPos = new(Main.MouseScreen.X + collumn, Main.MouseScreen.Y + row + YOffset);

			// draw slot background
			const float BackgroundScale = 0.7f;
			Vector2 backgroundPos = new(drawPos.X - MaxSize + 2, drawPos.Y - MaxSize + 2);
			spriteBatch.DrawWithScale(TextureAssets.InventoryBack.Value, backgroundPos, BackgroundScale);

			// handle animation frames
			int frameCount = 1;
			Rectangle itemFrameRect = itemTexture.Frame();
			if (Main.itemAnimations[items[i].type] != null) {
				itemFrameRect = Main.itemAnimations[items[i].type].GetFrame(itemTexture);
				frameCount = Main.itemAnimations[items[i].type].FrameCount;
			}

			// handle draw scale
			float drawScale = 1f;
			if (itemTexture.Width > MaxSize || itemTexture.Height / frameCount > MaxSize) {
				drawScale = MaxSize / (float)(itemFrameRect.Width <= itemFrameRect.Height ?
					itemFrameRect.Height :
					itemFrameRect.Width);
			}

			// draw item texture
			Vector2 itemPos = drawPos - itemFrameRect.Size() * drawScale / 2;
			spriteBatch.DrawWithScale(itemTexture, itemPos, itemFrameRect, drawScale);

			// draw stack text
			if (items[i].stack > 1) {
				Vector2 textPos = drawPos - new Vector2(10, 0);
				Utils.DrawBorderString(spriteBatch, items[i].stack.ToString(), textPos, Color.White, Main.inventoryScale);
			}

			// go to next slot
			collumn += MaxSize + Padding;
		}
	}
}
