using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace BetterChests.src.Edits;

internal class OpenChestEdits
{
	public static void Load()
	{
		On.Terraria.Chest.IsPlayerInChest += QuckStackAllowOpenChests;
		On.Terraria.Chest.UsingChest += AllowEnterOpenChests;
		IL.Terraria.UI.ItemSlot.LeftClick_ItemArray_int_int += SyncChests;
	}

	private static void SyncChests(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			GOAL: Send new chest info to server when player clicks into a chest slot.

			C#:
				if (context == 3 && Main.netMode == 1) {
					<--- here
					NetMessage.SendData(MessageID.SyncChestItem, -1, -1, null, player.chest, slot);
				}

			IL:
				IL_0300: ldarg.1
				IL_0301: ldc.i4.3
				IL_0302: bne.un    IL_0BC8

				IL_0307: ldsfld    int32 Terraria.Main::netMode
				IL_030C: ldc.i4.1
				IL_030D: bne.un    IL_0BC8
				<--- here
		*/

		c.GotoNext(MoveType.After,
			i => i.MatchLdarg(1),
			i => i.MatchLdcI4(3),
			i => i.MatchBneUn(out _),
			i => i.MatchLdsfld<Main>("netMode"),
			i => i.MatchLdcI4(1),
			i => i.MatchBneUn(out _));

		c.EmitDelegate(() =>
		{
			ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
			packet.Write((byte)0);
			packet.Write("Hello world!");
			packet.Send();
		});
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
