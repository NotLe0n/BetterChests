using BetterChests.src.UIElements;
using BetterChests.src.Edits;
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

	[Label("Default sort option")]
	[Tooltip("Changes the functionality of the \"Sort Items\" button")]
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

	public override void OnChanged()
	{
		ILEdits.CurrentSortFunction = defaultChestSortOptions == null ? "Default sort" : defaultChestSortOptions.selection;
		ILEdits.DisableConfirmationButton = disableConfirmationButton;
		base.OnChanged();
	}
}

internal class OptionSelectionPair<T>
{
	public T selection;
	public T[] options;

	public override bool Equals(object obj)
	{
		if (obj is OptionSelectionPair<T> otherPair)
		{
			return otherPair.selection.Equals(selection) && otherPair.options.Equals(options);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return new { selection, options }.GetHashCode();
	}
}
