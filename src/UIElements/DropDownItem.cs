using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests.UIElements;

internal class DropDownItem<T> : UIElement
{
	private readonly T name;
	private event UIElementAction OnSelect;
	public T Name => name;
	public new bool IsMouseHovering { get; private set; }

	public DropDownItem(T name, UIElementAction onSelectAction)
	{
		this.name = name;
		OnSelect = onSelectAction;
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);
		CalculatedStyle dimensions = GetDimensions();

		// draw Background
		Color bgColor = IsMouseHovering ? new Color(63, 82, 151) * 0.8f : new Color(39, 58, 127) * 0.7f;

		spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimensions.ToRectangle(), Color.Black * 0.8f);
		spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)dimensions.X + 2, (int)dimensions.Y + 2, (int)dimensions.Width - 4, (int)dimensions.Height - 4), bgColor);
		
		// draw name
		spriteBatch.DrawString(FontAssets.MouseText.Value, name.ToString(), new Vector2(dimensions.X + 5, dimensions.Y + 1), Color.White);
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		Rectangle dimensions = GetDimensions().ToRectangle();

		// Play sound once on hover
		IsMouseHovering = dimensions.Contains(Main.MouseScreen.ToPoint());
		if (IsMouseHovering && !dimensions.Contains(Main.lastMouseX, Main.lastMouseY)) {
			SoundEngine.PlaySound(SoundID.MenuTick);
		}

		// On click
		if (IsMouseHovering && Main.mouseLeft && !Main.mouseLeftRelease) {
			OnSelect?.Invoke(this);
		}
	}
}
