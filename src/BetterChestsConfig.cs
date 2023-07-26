using BetterChests.Edits;
using BetterChests.UIElements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BetterChests;

#pragma warning disable CS0649 // field is never assigned to

internal class BetterChestsConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;
	
	[DefaultValue(false)]
	public bool disableConfirmationButton;

	
	[JsonDefaultValue(
@"{
'selection': 'Default sort',
'options': [
    'Default sort',
    'Sort by ID',
    'Sort alphabetically',
    'Sort by rarity',
    'Sort by stack size',
    'Sort by value',
    'Sort by damage',
    'Sort by defense',
    'Sort randomly'
    ]
}"
	)]
	[CustomModConfigItem(typeof(DropDownMenu<string>))]
	public OptionSelectionPair<string> defaultChestSortOptions;
	
	[DefaultValue(false)]
	public bool disableChestHover;

	public override void OnChanged()
	{
		ChestButtonEdits.CurrentSortFunction = defaultChestSortOptions.selection;
		ChestButtonEdits.DisableConfirmationButton = disableConfirmationButton;
		base.OnChanged();
	}
}

[Serializable]
internal struct OptionSelectionPair<T>
{
	public T selection;
	public T[] options;
}
