using Terraria.ModLoader;
using BetterChests.src.Edits;

namespace BetterChests.src;

public class BetterChests : Mod
{
	public override void Load()
	{
		ILEdits.Load();
	}
}
