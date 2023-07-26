using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.UI.States;

namespace BetterChests.Edits;

internal class ChestNameEdits
{
	private const int NewMaxChestLength = 80;

	public static void Load()
	{
		IL_UIVirtualKeyboard.ctor += AllowBiggerChestName_Client;
		Terraria.IL_MessageBuffer.GetData += AllowBiggerChestName_GetData;
		Terraria.IL_NetMessage.SendData += AllowBiggerChestName_SendData;
	}

	private static void AllowBiggerChestName_SendData(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C#:
				before:
					string text3 = text.ToString();
					num12 = (byte)text3.Length;
					if (num12 == 0 || num12 > 20)
						num12 = 255;
					else
						text2 = text3;
				after:
					if (num12 == 0 || num12 > 20)
			IL:
				IL_1b85: ldloc.s 68
				IL_1b87: brfalse.s IL_1b8f
				IL_1b89: ldloc.s 68
				IL_1b8b: ldc.i4.s 20		<==== change this
					<=== here
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Brfalse_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdcI4(20)
		)) throw new("AllowBiggerChestName_SendData edit failed!");

		c.Prev.Operand = NewMaxChestLength;
	}

	private static void AllowBiggerChestName_Client(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			GOAL: Allow the user to write a chest name longer than 20 chars.
				  There is a const Chest::MaxNameLength, but const names don't get compiled into IL

			C#:
				int textMaxLength = _edittingSign ? 1200 : 20;

			IL:
				IL_0ACF: ldarg.0
				IL_0AD0: ldfld     bool Terraria.GameContent.UI.States.UIVirtualKeyboard::_edittingSign
				IL_0AD5: brtrue.s IL_0ADB
				IL_0AD7: ldc.i4.s  20

		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(0),
			i => i.MatchLdfld<UIVirtualKeyboard>("_edittingSign"),
			i => i.MatchBrtrue(out _),
			i => i.MatchLdcI4(20)
				)) throw new("AllowBiggerChestName_Client edit failed!");

		c.Prev.Operand = NewMaxChestLength;
	}

	private static void AllowBiggerChestName_GetData(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			NOTES:
				GetData() case 33 docs:
					chestIndex, x, y, nameLen, name
					name is only sent if nameLen <= 20
					on client, does the open and close sounds (if changed)
					on server, sends ChestName and SyncPlayerChestIndex to other clients

			GOAL: Allow the user to write a chest name longer than 20 chars.
				  There is a const Chest::MaxNameLength, but const names don't get compiled into IL

			C#:
				before:
					string name = string.Empty;
					if (num74 != 0) {
						if (num74 <= 20) {
							name = reader.ReadString();
						}

				after:
					string name = string.Empty;
					if (num74 != 0) {
						if (num74 <= newMaxChestLength) {
							name = reader.ReadString();
						}
			IL:
				IL_44ca: ldsfld string [System.Runtime]System.String::Empty
				IL_44cf: stloc.s 207

				IL_44d1: ldloc.s 206
				IL_44d3: brfalse.s IL_4478

				IL_44d5: ldloc.s 206
				IL_44d7: ldc.i4.s 20	<=== change this

				<---- here
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsfld<string>("Empty"),
			i => i.Match(OpCodes.Stloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Brfalse_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdcI4(20)
		)) {
			throw new("AllowBiggerChestName_GetData edit failed!");
		}

		c.Prev.Operand = NewMaxChestLength;
	}
}
