using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace BetterChests.src;

public enum SortOptionsMode
{
	Chest,
	Inventory
}

public static class NewItemSorting
{
	// default sort overload
	public static void SortByMode(bool reversed, SortOptionsMode mode)
	{
		switch (mode)
		{
			case SortOptionsMode.Chest:
				DefaultChestSort(reversed);
				break;
			case SortOptionsMode.Inventory:
				DefaultInventorySort(reversed);
				break;
		}
	}

	public static void SortByMode<T>(Func<Item, T> func, bool reversed, SortOptionsMode mode)
	{
		switch (mode)
		{
			case SortOptionsMode.Chest:
				SortChest(func, reversed);
				break;
			case SortOptionsMode.Inventory:
				SortInventory(func, reversed);
				break;
		}
	}

	// Inventory Sorting

	public static void DefaultInventorySort(bool reversed)
	{
		ItemSorting.SortInventory();

		if (reversed)
		{
			ref var items = ref Main.LocalPlayer.inventory;
			var reversedItems = items.Where((_, index) => index is > 9 and < 50); // do not reverse hotbar or money/ammo slots

			reversedItems = reversedItems
				.Reverse() // reverse order
				.OrderBy(x => x.IsAir); // air always goes last

			// insert sorted list
			items = items[0..10].Concat(reversedItems).Concat(items[50..]).ToArray();
		}
	}

	public static void SortInventory<T>(Func<Item, T> func, bool reversed)
	{
		ref var items = ref Main.LocalPlayer.inventory;
		var sortedItems = items.Where((_, index) => index is > 9 and < 50);

		if (!reversed)
		{
			// order the items according to the function.
			sortedItems = sortedItems.OrderBy(func).ThenBy(x => x.type);
		}
		else
		{
			// order the items according to the function in reversed order.
			sortedItems = sortedItems.OrderByDescending(func).ThenByDescending(x => x.type);
		}

		// air always goes last
		sortedItems = sortedItems.OrderBy(x => x.IsAir);

		// insert sorted list
		sortedItems = items[0..10].Concat(sortedItems).Concat(items[50..]);

		// add glow to changed items
		AddRandomGlow(items, sortedItems.ToArray(), false);

		// Apply changes
		items = sortedItems.ToArray();
	}

	// Chest Sorting

	public static void DefaultChestSort(bool reversed)
	{
		ItemSorting.SortChest();

		if (reversed)
		{
			// all items in the chest
			ref var items = ref GetChestItems();
			
			var reversedItems = items.Reverse() // reverse order
				.OrderBy(x => x.IsAir); // air always goes last

			// Apply changes
			items = reversedItems.ToArray();

			// sync chest contents with all clients
			SyncChest(items);
		}
	}

	public static void SortChest<T>(Func<Item, T> func, bool reversed)
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

		// add glow to changed items
		AddRandomGlow(items, sortedItems, true);

		// Apply changes
		items = sortedItems;

		// sync chest contents with all clients
		SyncChest(items);
	}

	private static void AddRandomGlow(Item[] items, Item[] sortedItems, bool chest)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (!sortedItems[i].IsAir && items[i] != sortedItems[i])
			{
				// Change color of changed slots
				ItemSlot.SetGlow(i, Main.rand.NextFloat(), chest);
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

	private static void SyncChest(Item[] items)
	{
		// sync chest contents with all clients
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			for (int i = 0; i < items.Length; i++)
			{
				NetMessage.SendData(MessageID.SyncChestItem, number: Main.LocalPlayer.chest, number2: i);
			}
		}
	}
}
