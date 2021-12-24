using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BetterChests.src;

#pragma warning disable CS0649 // field is never assigned to

internal class BetterChestsConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[Label("Disable Deposit All/Loot All Confirmation message")]
	[DefaultValue(false)]

	public bool disableConfirmationButton;

	public override void OnChanged()
	{
		ILEdits.disableConfirmationButton = disableConfirmationButton;
		base.OnChanged();
	}
}
