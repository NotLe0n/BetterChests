using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests;

internal class Hotkeys : ModPlayer
{
	public static ModKeybind DepositAll, LootAll, QuickStack, Restock, SortChest, SortInventory;

	public override void ProcessTriggers(TriggersSet triggersSet)
	{
		base.ProcessTriggers(triggersSet);

		if (DepositAll.JustPressed && Player.chest != -1) {
			ChestUI.DepositAll(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
		}
		if (LootAll.JustPressed && Player.chest != -1) {
			ChestUI.LootAll();
		}
		if (QuickStack.JustPressed) {
			Player.QuickStackAllChests();
			ChestUI.QuickStack(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
		}
		if (Restock.JustPressed) {
			ChestUI.Restock();
		}
		if (SortChest.JustPressed) {
			ItemSorting.SortChest();
		}
		if (SortInventory.JustPressed) {
			ItemSorting.SortInventory();
		}
	}
}

internal class HotkeyLoader : ModSystem
{
	public override void Load()
	{
		Hotkeys.DepositAll = KeybindLoader.RegisterKeybind(Mod, "Deposit All", Keys.None);
		Hotkeys.LootAll = KeybindLoader.RegisterKeybind(Mod, "Loot All", Keys.None);
		Hotkeys.QuickStack = KeybindLoader.RegisterKeybind(Mod, "Quick Stack", Keys.None);
		Hotkeys.Restock = KeybindLoader.RegisterKeybind(Mod, "Restock", Keys.None);
		Hotkeys.SortChest = KeybindLoader.RegisterKeybind(Mod, "Sort Chest", Keys.None);
		Hotkeys.SortInventory = KeybindLoader.RegisterKeybind(Mod, "Sort Inventory", Keys.None);

		base.Load();
	}

	public override void Unload()
	{
		Hotkeys.DepositAll = null;
		Hotkeys.LootAll = null;
		Hotkeys.QuickStack = null;
		Hotkeys.Restock = null;
		Hotkeys.SortChest = null;
		Hotkeys.SortInventory = null;

		base.Unload();
	}
}
