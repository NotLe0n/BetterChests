using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests.src.UIElements
{
    public class UITextOption : UIText
    {
        public float TextScale { get; set; }
        public bool isLarge;
        private float _firstTextScale;

        public UITextOption(string text, float textScale = 0.75f, bool large = false) : base(text, textScale, large)
        {
            _firstTextScale = textScale;
            TextScale = textScale;
            isLarge = large;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                if (TextScale <= 1)
                {
                    TextScale += 0.05f;
                }
            }
            else
            {
                if (TextScale >= _firstTextScale)
                {
                    TextScale -= 0.05f;
                }
            }

            SetText(Text, TextScale, isLarge);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}
