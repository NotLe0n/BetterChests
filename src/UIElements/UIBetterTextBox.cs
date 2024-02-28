using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace BetterChests.UIElements;

// original code from jopojelly and darthmorf. Modified to suit my needs
internal class UIBetterTextBox : UIPanel
{
	private int maxLength = int.MaxValue;
	private bool focused;
	private string currentString;
	private int textBlinkerCount;
	private int textBlinkerState;
	private readonly string hintText;

	public event Action? OnFocus;
	public event Action? OnUnfocus;
	public event Action? OnTextChanged;
	public event Action? OnTabPressed;
	public event Action? OnEnterPressed;
	public event Action<Keys>? OnKeyPressed;

	internal bool unfocusOnEnter = true;
	internal bool unfocusOnTab = true;
	internal bool unfocusOnEsc = true;

	public Color TextColor { get; set; }
	public string Text => currentString;
	public int MaxLength { get => maxLength; set => maxLength = value; }
	public bool IsFocused => focused;

	public UIBetterTextBox(string hintText)
	{
		SetPadding(0);
		TextColor = Color.White;
		BackgroundColor = new Color(63, 82, 151) * 0.7f;
		BorderColor = Color.Black;

		focused = false;
		currentString = string.Empty;
		this.hintText = hintText;
	}

	public override void LeftClick(UIMouseEvent evt)
	{
		Focus();
		base.LeftClick(evt);
	}

	public void Unfocus()
	{
		// exit if it's already unfocused
		if (!focused) return;

		focused = false;
		Main.blockInput = false;

		OnUnfocus?.Invoke();
	}

	public void Focus()
	{
		// exit if it's already focused
		if (focused) return;

		focused = true;
		Main.clrInput();
		Main.blockInput = true;

		OnFocus?.Invoke();
	}

	public override void Update(GameTime gameTime)
	{
		if (!ContainsPoint(Main.MouseScreen) && (Main.mouseLeft || Main.mouseRight)) //This solution is fine, but we need a way to cleanly "unload" a UIElement
		{
			//TODO, figure out how to refocus without triggering unfocus while clicking enable button.
			Unfocus();
		}

		// prevent items being used when clicked on the search bar
		if (ContainsPoint(Main.MouseScreen)) {
			Main.LocalPlayer.mouseInterface = true;
		}
		base.Update(gameTime);
	}

	public void SetText(string text)
	{
		// keep text length under the maximum text length
		if (text.Length > maxLength)
			return;

		// don't invoke event if the string hasn't changed
		if (currentString == text)
			return;

		currentString = text;
		OnTextChanged?.Invoke();
	}

	private static bool JustPressed(Keys key)
	{
		return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		// draw panel
		base.DrawSelf(spriteBatch);

		if (focused) {
			Terraria.GameInput.PlayerInput.WritingText = true;
			Main.instance.HandleIME();

			// set new input
			SetText(Main.GetInputText(currentString));

			// if any keys are currently pressed (this includes special/invisible keys)
			if (Main.inputText.GetPressedKeys().Length > 0) {
				OnKeyPressed?.Invoke(Main.inputText.GetPressedKeys()[0]);
			}

			// special character: TAB
			if (JustPressed(Keys.Tab)) {
				if (unfocusOnTab) Unfocus();
				OnTabPressed?.Invoke();
			}

			// special character: ENTER
			if (JustPressed(Keys.Enter)) {
				Main.drawingPlayerChat = false;
				if (unfocusOnEnter) Unfocus();
				OnEnterPressed?.Invoke();
			}

			// special character: ESC
			if (unfocusOnEsc && JustPressed(Keys.Escape)) {
				Unfocus();
			}

			// increase and reset cursor bink counter
			if (++textBlinkerCount >= 20) {
				textBlinkerState = (textBlinkerState + 1) % 2;
				textBlinkerCount = 0;
			}

			Main.instance.DrawWindowsIMEPanel(new Vector2(98f, Main.screenHeight - 36), 0f);
		}

		DrawText(spriteBatch);
	}

	private void DrawText(SpriteBatch spriteBatch)
	{
		// add cursor bar
		string displayString = currentString;
		if (textBlinkerState == 1 && focused) {
			displayString += "|";
		}

		var drawPos = GetDimensions().Position() + new Vector2(4, 2);
		if (currentString.Length == 0 && !focused) {
			// draw hint text
			spriteBatch.DrawString(FontAssets.MouseText.Value, hintText, drawPos, TextColor * 0.5f);
		}
		else {
			// draw text
			spriteBatch.DrawString(FontAssets.MouseText.Value, displayString, drawPos, TextColor);
		}
	}

	// make sure to unfocus the searchbar when it deactivates
	public override void OnDeactivate()
	{
		Unfocus();
		base.OnDeactivate();
	}
}
