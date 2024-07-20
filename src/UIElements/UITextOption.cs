using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests.UIElements;

public class UITextOption : UIText
{
	public float TextScale { get; set; }
	public Color NewTextColor { get => newTextColor; set => TextColor = newTextColor = value; }
	public bool isLarge;

	private Color newTextColor;
	private readonly float firstTextScale;

	public UITextOption(string text, float textScale = 0.75f, bool large = false) : base(text, textScale, large)
	{
		NewTextColor = Color.White;

		firstTextScale = textScale;
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

		if (IsMouseHovering) {
			if (newTextColor == Color.White)
				TextColor = Main.OurFavoriteColor;

			if (TextScale <= 1) {
				TextScale += 0.05f;
			}
		}
		else {
			if (newTextColor == Color.White)
				TextColor = Color.White;

			if (TextScale > firstTextScale) {
				TextScale -= 0.05f;
			}
		}

		SetText(Text, TextScale, isLarge);
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		if (ContainsPoint(Main.MouseScreen)) {
			Main.LocalPlayer.mouseInterface = true;
		}
	}
}
