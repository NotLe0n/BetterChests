using Humanizer;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BetterChests.Edits;

internal class OpenChestEdits
{
	private static readonly OwnershipSystem OwnershipSystem = ModContent.GetInstance<OwnershipSystem>();
	
	public static void Load()
	{
		On_Chest.IsPlayerInChest += QuickStackIsPlayerInChest;
		On_Chest.UsingChest += AllowEnterOpenChests;
		Terraria.UI.On_ChestUI.DrawSlots += SyncChest;
		On_Player.TileInteractionsUse += On_PlayerOnTileInteractionsUse;
		IL_Player.TileInteractionsUse += IL_PlayerOnTileInteractionsUse;
	}

	private static void IL_PlayerOnTileInteractionsUse(ILContext il)
	{
		var c = new ILCursor(il);
		
		/*
			C#:
				...
				
			[+]	OpenChest(num45, num46);

				if (Main.netMode == 1) {
					if (num45 == chestX && num46 == chestY && chest != -1) {
						chest = -1;
						Recipe.FindRecipes();
						SoundEngine.PlaySound(11);
					}
				
				...
				
			IL:
				...
					<=== here
				IL_1CE4: ldsfld    int32 Terraria.Main::netMode
				IL_1CE9: ldc.i4.1
				IL_1CEA: bne.un.s  IL_1D5B

				IL_1CEC: ldloc.s   num45
				IL_1CEE: ldarg.0
				IL_1CEF: ldfld     int32 Terraria.Player::chestX
				IL_1CF4: bne.un.s  IL_1D30

				IL_1CF6: ldloc.s   num46
				...
		*/
		
		int chestX = -1;
		int chestY = -1;
		if (!c.TryGotoNext(MoveType.Before,
			    i => i.MatchLdsfld<Main>("netMode"),
			    i => i.MatchLdcI4(1),
			    i => i.Match(OpCodes.Bne_Un_S),
			    i => i.MatchLdloc(out chestX),
			    i => i.MatchLdarg0(),
			    i => i.MatchLdfld<Player>("chestX"),
			    i => i.Match(OpCodes.Bne_Un_S),
			    i => i.MatchLdloc(out chestY)
		    )) {
			throw new("IL Edit exception at OpenChestEdits::IL_PlayerOnTileInteractionsUse");
		}
		
		c.Index++;
		
		c.EmitLdloc(chestX);
		c.EmitLdloc(chestY);
		c.EmitDelegate((int x, int y) => ChestLeftClicked(BetterChests.GetChest(x, y)));
		
		/*
			C#:
				...
			
			[+] OpenChest(num65, num66);	
				
				bool flag13 = Chest.IsLocked(num65, num66);
				if (Main.netMode == 1 && num64 == 0 && !flag13) {
				
				... 
			
			IL:
				...
				IL_2408: ldloc.s   num65
				IL_240A: ldloc.s   num66
				IL_240C: call      bool Terraria.Chest::IsLocked(int32, int32)
				IL_2411: stloc.s   flag13
				IL_2413: ldsfld    int32 Terraria.Main::netMode
				IL_2418: ldc.i4.1
				...
		 
		*/


		if (!c.TryGotoNext(MoveType.Before,
			    i => i.MatchLdloc(out chestX),
			    i => i.MatchLdloc(out chestY),
			    i => i.MatchCall<Chest>("IsLocked"),
			    i => i.Match(OpCodes.Stloc_S),
			    i => i.MatchLdsfld<Main>("netMode"),
			    i => i.MatchLdcI4(1)
		    )) {
			throw new("IL Edit exception at OpenChestEdits::IL_PlayerOnTileInteractionsUse");
		}
		
		c.Index++;
		
		c.EmitLdloc(chestX);
		c.EmitLdloc(chestY);
		c.EmitDelegate((int x, int y) => ChestLeftClicked(Chest.FindChest(x, y)));
	}

	private static void ChestLeftClicked(int chestID)
	{
		if (chestID == -1) {
			UISystem.CloseChestUI();
			return;
		}
		
		UISystem.OpenChestUI(chestID);
	}

	private static void On_PlayerOnTileInteractionsUse(On_Player.orig_TileInteractionsUse orig, Player self, int myx, int myy)
	{
		if (TileID.Sets.BasicChest[Main.tile[myx, myy].TileType] || Main.tile[myx, myy].TileType == TileID.Dressers) {
			int chest = BetterChests.GetChest(myx, myy);
			if (chest != -1 && OwnershipSystem.IsNotOwner(chest, self.name)) {
				return;
			}
		}

		orig(self, myx, myy);
	}

	private static Item[] prevItems;
	public static bool serverUpdateRecieved;
	private static void SyncChest(Terraria.UI.On_ChestUI.orig_DrawSlots orig, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
	{
		orig(spriteBatch);

		// only check and send changes if playing in multiplayer and inside a chest
		if (Main.netMode != NetmodeID.MultiplayerClient || Main.player[Main.myPlayer].chest < 0)
			return;

		Item[] items = Main.chest[Main.player[Main.myPlayer].chest].item;

		// initialize prevItems and set it when this client recieved data from the server
		if (prevItems == null || serverUpdateRecieved) {
			prevItems = CloneItemArray(items);
			serverUpdateRecieved = false;
		}

		// go through all items and check if there were any changes, if so send the changes to the server
		bool changed = false;
		for (int i = 0; i < Chest.maxItems; i++) {
			// check if item has changed
			if (items[i].IsNotSameTypePrefixAndStack(prevItems[i])) {
				changed = true; // item has changed

				// send packet with slot id and item data to server
				ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
				packet.Write(BetterChests.ChestUpdatePacketID); // message id
				packet.Write(Main.player[Main.myPlayer].chest);
				packet.Write(i); // slot id
				ItemIO.Send(items[i], packet, true); // item data
				packet.Send();
			}
		}

		if (changed) {
			prevItems = CloneItemArray(items);
			BetterChests.dontUpdateMe = true; // dont apply change again
		}
	}

	// creates a clone of an item array
	private static Item[] CloneItemArray(Item[] arrayToClone)
	{
		var cloned = new Item[arrayToClone.Length];
		for (int i = 0; i < Chest.maxItems; i++) {
			cloned[i] = arrayToClone[i].Clone();
		}
		return cloned;
	}

	private static int AllowEnterOpenChests(On_Chest.orig_UsingChest orig, int i)
	{
		return -1; // no player is currently in this chest
	}

	private static bool QuickStackIsPlayerInChest(On_Chest.orig_IsPlayerInChest orig, int i)
	{
		if (OwnershipSystem.IsNotOwner(i, Main.LocalPlayer.name)) {
			return true;
		}

		return false; // no player is currently in this chest
	}
}
