using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using System.Collections.Generic;

namespace BetterChests.src;

public class NewItemSorting
{
	// TODO: Add InventorySort

	public static void DefaultSort(bool reversed)
	{
		ItemSorting.SortChest();

		if (reversed)
		{
			// all items in the chest
			ref var items = ref GetChestItems();
			IEnumerable<Item> reversedItems = items.Where(x => !x.IsAir);

			reversedItems = reversedItems.Reverse(); // reverse order

			// add air back
			int padLength = items.Length - reversedItems.Count();
			for (int i = 0; i < padLength; i++)
			{
				reversedItems = reversedItems.Append(new Item(0));
			}

			items = reversedItems.ToArray();
		}
	}

	public static void Sort<T>(Func<Item, T> func, bool reversed)
	{
		// all items in the chest
		ref var items = ref GetChestItems();

		// order the items according to the function.
		var sortedItems = items.OrderBy(func).ThenBy(x => x.type).ToArray();

		if (reversed)
		{
			// reverse the order
			sortedItems = sortedItems.Reverse().ToArray();
		}

		// Air always goes last
		sortedItems = sortedItems.OrderBy(x => x.IsAir).ToArray();

		for (int i = 0; i < items.Length; i++)
		{
			if (!sortedItems[i].IsAir && items[i] != sortedItems[i])
			{
				// Change color of changed slots
				ItemSlot.SetGlow(i, Main.rand.NextFloat(), true);
			}
		}

		// Apply changes
		items = sortedItems;

		// sync chest contents with all clients
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			for (int i = 0; i < items.Length; i++)
			{
				NetMessage.SendData(MessageID.SyncChestItem, number: Main.LocalPlayer.chest, number2: i);
			}
		}
	}

	private static ref Item[] GetChestItems()
	{
		switch (Main.LocalPlayer.chest)
		{
			case -2:
				return ref Main.LocalPlayer.bank.item;
			case -3:
				return ref Main.LocalPlayer.bank2.item;
			case -4:
				return ref Main.LocalPlayer.bank3.item;
			case -5:
				return ref Main.LocalPlayer.bank4.item;
			default:
				return ref Main.chest[Main.LocalPlayer.chest].item;
		}
	}
}
