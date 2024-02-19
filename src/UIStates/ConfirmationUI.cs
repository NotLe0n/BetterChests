using BetterChests.UIElements;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace BetterChests.UIStates;

internal class ConfirmationUI : UIState
{
	private readonly int buttonID;

	public ConfirmationUI(int buttonID, Action onClick)
	{
		this.buttonID = buttonID;

		float topOffset = buttonID switch {
			ChestUI.ButtonID.LootAll => 30,
			ChestUI.ButtonID.DepositAll => 55,
			ChestUI.ButtonID.Sort => 135,
			_ => 0
		};
		
		var confirmation = new UITextOption(Language.GetTextValue("Mods.BetterChests.Confirmation")) {
			NewTextColor = Color.Red,
			Top = new(Main.instance.invBottom + topOffset, 0),
			Left = new(506, 0) // magic number because vanilla does it the same way lmao
		};
		confirmation.OnLeftClick += (_, _) => onClick();
		Append(confirmation);
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		ChestUI.ButtonScale[buttonID] = 0f;
	}
}
