using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BetterChests.src.Edits;

internal class OpenChestEdits
{
	public static void Load()
	{
		On.Terraria.Chest.IsPlayerInChest += QuckStackAllowOpenChests;
		On.Terraria.Chest.UsingChest += AllowEnterOpenChests;
		On.Terraria.UI.ChestUI.DrawSlots += SyncChest;
	}

	private static Item[] prevItems;
	public static bool serverUpdateRecieved;
	private static void SyncChest(On.Terraria.UI.ChestUI.orig_DrawSlots orig, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
	{
		orig(spriteBatch);

		// only check and send changes if playing in multiplayer and inside a chest
		if (Main.netMode != NetmodeID.MultiplayerClient || Main.player[Main.myPlayer].chest < 0)
			return;	

		Item[] items = Main.chest[Main.player[Main.myPlayer].chest].item;

		// initialize prevItems and set it when this client recieved data from the server
		if (prevItems == null || serverUpdateRecieved)
		{
			prevItems = CloneItemArray(items);
			serverUpdateRecieved = false;
		}

		// go through all items and check if there were any changes, if so send the changes to the server
		bool changed = false;
		for (int i = 0; i < Chest.maxItems; i++)
		{
			// check if item has changed
			if (items[i].IsNotSameTypePrefixAndStack(prevItems[i]))
			{
				changed = true; // item has changed

				// send packet with slot id and item data to server
				ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
				packet.Write((byte)0); // message id
				packet.Write(i); // slot id
				ItemIO.Send(items[i], packet, true); // item data
				packet.Send();
			}
		}

		if (changed)
		{
			prevItems = CloneItemArray(items);
			BetterChests.dontUpdateMe = true; // dont apply change again
		}
	}

	// creates a clone of an item array
	private static Item[] CloneItemArray(Item[] arrayToClone)
	{
		var cloned = new Item[arrayToClone.Length];
		for (int i = 0; i < Chest.maxItems; i++)
		{
			cloned[i] = arrayToClone[i].Clone();
		}
		return cloned;
	}

	private static int AllowEnterOpenChests(On.Terraria.Chest.orig_UsingChest orig, int i)
	{
		return -1; // no player is currently in this chest
	}

	private static bool QuckStackAllowOpenChests(On.Terraria.Chest.orig_IsPlayerInChest orig, int i)
	{
		return false; // no player is currently in this chest
	}
}
