using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests.src.UIStates;

internal class ChestHoverUI : UIState
{
	public readonly Chest chest;

	public ChestHoverUI(Chest chest)
	{
		this.chest = chest;
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);

		if (chest == null)
			return;

		// all items in the chest
		Item[] items = chest.item.Where(x => x != null && x.type != ItemID.None).ToArray();

		int collumn = 0;
		int row = 20;
		int padding = 10;
		int maxSize = 20;
		for (int i = 0; i < items.Length; i++)
		{
			// set positions (10 items per row)
			if (i % 10 == 0)
			{
				row += maxSize + padding;
				collumn = maxSize;
			}

			// draw item
			Main.instance.LoadItem(items[i].type); // load item before trying to get its texture (Item only gets loaded once)
			Texture2D itemTexture = TextureAssets.Item[items[i].type].Value; // get item texture
			float drawScale = 1f;
			int frameCount = 1;
			Rectangle? rect = null;
			Vector2 drawPos = new Vector2((int)Main.MouseScreen.X + collumn, (int)Main.MouseScreen.Y + row);

			// handle animation frames
			if (Main.itemAnimations[items[i].type] != null)
			{
				rect = Main.itemAnimations[items[i].type].GetFrame(itemTexture);
				frameCount = Main.itemAnimations[items[i].type].FrameCount;
			}

			if (itemTexture.Width > maxSize || itemTexture.Height / frameCount > 18)
				drawScale = maxSize / (float)TextureAssets.Item[items[i].type].Width();

			// draw texture
			spriteBatch.Draw(itemTexture, drawPos, rect, Color.White, 0, Vector2.Zero, drawScale, SpriteEffects.None, 0f);

			// draw stack text
			if (items[i].stack != 1)
			{
				string text = items[i].stack.ToString();
				Vector2 pos = drawPos + new Vector2(0, itemTexture.Height / frameCount / 2);
				Utils.DrawBorderString(spriteBatch, text, pos, Color.White, 0.75f);
			}

			collumn += maxSize + padding;
		}
	}
}
