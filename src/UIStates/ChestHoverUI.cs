using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests.src.UIStates
{
    class ChestHoverUI : UIState
    {
        public static Chest chest;
        public static bool visible;

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (chest == null)
                return;

            Item[] items = chest.item.Where(x => x.type != ItemID.None).ToArray();

            int collumn = 0;
            int row = 20;
            int padding = 10;
            int maxSize = 20;
            for (int i = 0; i < items.Length; i++)
            {
                if (i % 10 == 0)
                {
                    row += maxSize + padding;
                    collumn = maxSize;
                }

                // draw item
                Texture2D itemTexture = Main.itemTexture[items[i].type];
                float drawScale = 1f;
                int frameCount = 1;
                Rectangle? rect = null;
                Vector2 drawPos = new Vector2((int)Main.MouseScreen.X + collumn, (int)Main.MouseScreen.Y + row);

                if (Main.itemAnimations[items[i].type] != null)
                {
                    rect = Main.itemAnimations[items[i].type].GetFrame(itemTexture);
                    frameCount = Main.itemAnimations[items[i].type].FrameCount;
                }

                if (itemTexture.Width > maxSize || itemTexture.Height / frameCount > 18)
                    drawScale = (float)maxSize / (float)Main.itemTexture[items[i].type].Width;

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

            visible = false;
        }
    }
}
