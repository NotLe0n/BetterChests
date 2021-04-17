using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests
{
    public class UITextOption : UIText
    {
        public float TextScale { get; set; }
        public bool isLarge;
        private float _firstTextScale;

        public UITextOption(string text, float textScale = 0.85f, bool large = false) : base(text, textScale, large)
        {
            _firstTextScale = textScale;
            TextScale = textScale;
            isLarge = large;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Main.PlaySound(SoundID.MenuTick);
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
    }
}
