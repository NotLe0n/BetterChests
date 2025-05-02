using BetterChests.Edits;
using BetterChests.UIElements;
using System.ComponentModel;
using Microsoft.Xna.Framework;
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
	public bool autoCloseSortingOptions;
	
	[DefaultValue(false)]
	public bool disableChestHover;
	
	[DefaultValue(false)]
	public bool disableSearchbar;

	[DefaultValue(false)]
	public bool disablePings;

	[Slider]
	[Range(1, 60)]
	[DefaultValue(15)]
	public int pingTime;

	[DefaultValue(typeof(Color), "255, 0, 0, 255")]
	[ColorHSLSlider(false)]
	[ColorNoAlpha]
	public Color pingHue;

	// Has to be the bottom most setting. TODO: Fix draw order
	[JsonDefaultValue(
        """
        {
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
        }
        """
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
