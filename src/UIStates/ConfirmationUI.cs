using BetterChests.src.UIElements;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace BetterChests.src.UIStates
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
			confirmation.OnClick += onclick;
			Append(confirmation);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			ChestUI.ButtonScale[buttonID] = 0f;
		}
	}
}
