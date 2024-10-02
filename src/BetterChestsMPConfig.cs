using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BetterChests;

public class BetterChestsMPConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;
	
    [DefaultValue(false)]
    public bool disableChestOwnership;
}