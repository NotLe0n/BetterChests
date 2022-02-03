using BetterChests.src.UIElements;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.UI;

namespace BetterChests.src.UIStates;

internal class ConfirmationUI : UIState
{
	private readonly int buttonID;

	public ConfirmationUI(int buttonID, float topOffset, Action onClick)
	{
		this.buttonID = buttonID;

		var confirmation = new UITextOption("Are you sure?")
		{
			NewTextColor = Color.Red,
			Top = new(Main.instance.invBottom + topOffset, 0),
			Left = new(506, 0) // magic number because vanilla does it the same way lmao
		};
		confirmation.OnClick += (evt, elm) => onClick();
		Append(confirmation);
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		ChestUI.ButtonScale[buttonID] = 0f;
	}
}
