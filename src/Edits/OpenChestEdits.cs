using Terraria;

namespace BetterChests.Edits;

internal static class OpenChestEdits
{
	public static void Load()
	{
		On_Chest.IsPlayerInChest += QuickStackIsPlayerInChest;
		On_Chest.UsingChest += AllowEnterOpenChests;
	}

	private static int AllowEnterOpenChests(On_Chest.orig_UsingChest orig, int i)
	{
		return -1; // no player is currently in this chest
	}
	
	private static bool QuickStackIsPlayerInChest(On_Chest.orig_IsPlayerInChest orig, int i)
	{
		return false; // no player is currently in this chest
	}
}
