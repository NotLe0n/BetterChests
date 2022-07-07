using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;
using ReLogic.Graphics;
using Terraria.Audio;
using Terraria.ID;
using Terraria;

namespace BetterChests.src.UIElements;

internal class DropDownItem<T> : UIElement
{
	private readonly T name;
	public T Name => name;
	public event UIElementAction OnSelect;
	public new bool IsMouseHovering { get; private set; }

	public DropDownItem(T name)
	{
		this.name = name;
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);
		CalculatedStyle dimensions = GetDimensions();

		// draw Background
		spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimensions.ToRectangle(), IsMouseHovering ? new Color(63, 82, 151) * 0.8f : new Color(39, 58, 127) * 0.7f);
		// draw name
		spriteBatch.DrawString(FontAssets.MouseText.Value, name.ToString(), new(dimensions.X + 5, dimensions.Y + 3), Color.White);
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		var dimensions = GetDimensions().ToRectangle();

		// Play sound once on hover
		IsMouseHovering = dimensions.Contains(Main.MouseScreen.ToPoint());
		if (IsMouseHovering && !dimensions.Contains(Main.lastMouseX, Main.lastMouseY)) {
			SoundEngine.PlaySound(SoundID.MenuTick);
		}

		// On click
		if (IsMouseHovering && Main.mouseLeft && !Main.mouseLeftRelease) {
			OnSelect.Invoke(this);
		}
	}
}
