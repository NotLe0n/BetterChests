using Microsoft.Xna.Framework;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterChests.Edits;

internal static class ChestOwnershipEdits
{
	private static readonly OwnershipSystem OwnershipSystem = ModContent.GetInstance<OwnershipSystem>();

	public static void Load()
	{
		On_Player.TileInteractionsUse += PreventChestOpen;
		IL_Player.TileInteractionsUse += AddOpenChestHook;
		IL_Chest.PutItemInNearbyChest += PreventQuickstack;
	}
	
	private static void PreventQuickstack(ILContext il)
	{
		var c = new ILCursor(il);
		
		/*
			IL:
				IL_003A: ldsfld    class Terraria.Chest[] Terraria.Main::chest
				IL_003F: ldloc.1
				IL_0040: ldelem.ref
				IL_0041: ldfld     int32 Terraria.Chest::y
				IL_0046: call      bool Terraria.Chest::IsLocked(int32, int32)
				IL_004B: brtrue    IL_01B3
					<== here
					
		*/

		ILLabel label = null!;
		int vari = -1;
		if (!c.TryGotoNext(MoveType.After, 
			    i => i.MatchLdsfld<Main>(nameof(Main.chest)),
			    i => i.MatchLdloc(out vari),
			    i => i.MatchLdelemRef(),
			    i => i.MatchLdfld<Chest>("y"),
			    i => i.MatchCall<Chest>(nameof(Chest.IsLocked)),
			    i => i.MatchBrtrue(out label!)
		)) {
			throw new($"IL Edit exception at {nameof(ChestOwnershipEdits)}::{nameof(PreventQuickstack)}");
		}
		
		c.EmitLdloc(vari);
		c.EmitLdarg1();
		c.EmitDelegate((int chest, Vector2 position) =>
		{
			Player player = Main.player.First(p => p.Center == position);
			return OwnershipSystem.IsNotOwner(chest, player.name);
		});
		c.EmitBrtrue(label);
	}

	private static void AddOpenChestHook(ILContext il)
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
			throw new($"IL Edit exception at {nameof(ChestOwnershipEdits)}::{nameof(AddOpenChestHook)}");
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
			throw new("IL Edit exception at OpenChestEdits::AddOpenChestHook");
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

	private static void PreventChestOpen(On_Player.orig_TileInteractionsUse orig, Player self, int myx, int myy)
	{
		if (TileID.Sets.BasicChest[Main.tile[myx, myy].TileType] || Main.tile[myx, myy].TileType == TileID.Dressers) {
			int chest = BetterChests.GetChest(myx, myy);
			if (chest != -1 && OwnershipSystem.IsNotOwner(chest, self.name)) {
				return;
			}
		}

		orig(self, myx, myy);
	}
}