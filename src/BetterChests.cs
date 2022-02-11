using Terraria.ModLoader;
using BetterChests.src.Edits;

namespace BetterChests.src;

public class BetterChests : Mod
{
	public override void Load()
	{
		// Opens the ChestHoverUI when hovering over a container
		ChestHoverEdits.Load();

		// Changes the functionality of buttons
		ChestButtonEdits.Load();

		// Increases the maximum chest name length
		ChestNameEdits.Load();

	}
}
