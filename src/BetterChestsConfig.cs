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
	public bool disableLootAllConfirmationButton;
	
	[DefaultValue(false)]
	public bool disableDepositAllConfirmationButton;
	
	[DefaultValue(true)]
	public bool disableSortConfirmationButton;

	[DefaultValue(false)]
	public bool disableChestHover;

	[DefaultValue(false)]
	public bool autoCloseSortingOptions;
	
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
		ChestButtonEdits.DisableDepositAllConfirmationButton = disableDepositAllConfirmationButton;
		ChestButtonEdits.DisableLootAllConfirmationButton = disableLootAllConfirmationButton;
		ChestButtonEdits.DisableSortConfirmationButton = disableSortConfirmationButton;
		ChestButtonEdits.AutoCloseSortOptions = autoCloseSortingOptions;

		base.OnChanged();
	}
}
