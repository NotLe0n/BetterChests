using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterChests.src;

internal static class UIUtils
{
	public static void DrawWithScale(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, float scale)
	{
		spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
	}

	public static void DrawWithScale(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? rect, float scale)
	{
		spriteBatch.Draw(texture, position, rect, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
	}
}
