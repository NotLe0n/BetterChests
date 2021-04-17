using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BetterChests.src
{
    internal class ConfirmationUI : UIState
    {
        public static bool visible = false;
        public int buttonID;
        public float topOffset;
        public MouseEvent onclick;

        public override void OnInitialize()
        {
            var confirmation = new UITextOption("Are you sure?");
            confirmation.TextColor = Color.Red;
            confirmation.Top.Set(Main.instance.invBottom + topOffset, 0);
            confirmation.Left.Set(506, 0); // magic number because vanilla does it the same way lmao
            confirmation.OnMouseDown += onclick;
            Append(confirmation);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ChestUI.ButtonScale[buttonID] = 0f;
        }
    }
}
