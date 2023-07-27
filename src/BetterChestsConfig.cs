using BetterChests.Edits;
using BetterChests.UIElements;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BetterChests;

#pragma warning disable CS0649 // field is never assigned to

internal class BetterChestsConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;
	
	[DefaultValue(false)]
	public bool disableConfirmationButton;

	[DefaultValue(false)]
	public bool disableChestHover;
	
	// Has to be the bottom most setting. TODO: Fix draw order
	[JsonDefaultValue(
@"{
'selection': 'Default',
'options': [
    'Default',
    'ID',
    'Alphabetically',
    'Rarity',
    'Stack',
    'Value',                                         
    'Damage',
    'Defense',
    'Random'
    ]
}"
	)]
	[CustomModConfigItem(typeof(DropDownMenu<SortOption>))]
	public OptionSelectionPair<SortOption> defaultChestSortOptions;

	public override void OnChanged()
	{
		ChestButtonEdits.CurrentSortFunction = defaultChestSortOptions.selection;
		ChestButtonEdits.DisableConfirmationButton = disableConfirmationButton;
		base.OnChanged();
	}
}
