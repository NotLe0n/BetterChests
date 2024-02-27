using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests;

internal class KeybindSystem : ModSystem
{
	public ModKeybind? depositAll, lootAll, quickStack, restock, sortChest, sortInventory;

	public override void Load()
	{
		depositAll = KeybindLoader.RegisterKeybind(Mod, "Deposit All", Keys.None);
		lootAll = KeybindLoader.RegisterKeybind(Mod, "Loot All", Keys.None);
		quickStack = KeybindLoader.RegisterKeybind(Mod, "Quick Stack", Keys.None);
		restock = KeybindLoader.RegisterKeybind(Mod, "Restock", Keys.None);
		sortChest = KeybindLoader.RegisterKeybind(Mod, "Sort Chest", Keys.None);
		sortInventory = KeybindLoader.RegisterKeybind(Mod, "Sort Inventory", Keys.None);

		base.Load();
	}

	public override void Unload()
	{
		depositAll = null;
		lootAll = null;
		quickStack = null;
		restock = null;
		sortChest = null;
		sortInventory = null;

		base.Unload();
	}
}

internal class KeybindPlayer : ModPlayer
{
	private static readonly KeybindSystem Keybinds = ModContent.GetInstance<KeybindSystem>();
	
	public override void ProcessTriggers(TriggersSet triggersSet)
	{
		base.ProcessTriggers(triggersSet);

		if (Keybinds.depositAll.JustPressed && Player.chest != -1) {
			ChestUI.DepositAll(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
		}
		if (Keybinds.lootAll.JustPressed && Player.chest != -1) {
			ChestUI.LootAll();
		}
		if (Keybinds.quickStack.JustPressed) {
			Player.QuickStackAllChests();
			ChestUI.QuickStack(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
		}
		if (Keybinds.restock.JustPressed) {
			ChestUI.Restock();
		}
		if (Keybinds.sortChest.JustPressed) {
			ItemSorting.SortChest();
		}
		if (Keybinds.sortInventory.JustPressed) {
			ItemSorting.SortInventory();
		}
	}
}