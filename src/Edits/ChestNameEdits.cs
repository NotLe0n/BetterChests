using MonoMod.Cil;
using Terraria.GameContent.UI.States;

namespace BetterChests.src.Edits;

internal class ChestNameEdits
{
	public const int newMaxChestLength = 80;

	public static void Load()
	{
		IL.Terraria.GameContent.UI.States.UIVirtualKeyboard.ctor += AllowBiggerChestNameClient;
		IL.Terraria.MessageBuffer.GetData += AllowBiggerChestNameServer;
	}

	private static void AllowBiggerChestNameClient(ILContext il)
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
				)) throw new("client chest name edit failed");

		c.Prev.Operand = newMaxChestLength;
	}

	private static void AllowBiggerChestNameServer(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			GOAL: Allow the user to write a chest name longer than 20 chars.
				  There is a const Chest::MaxNameLength, but const names don't get compiled into IL

			C#:
				string name = string.Empty;
				if (num74 != 0) {
					if (num74 <= 20)
						name = reader.ReadString();
				...
				}

			IL:
				IL_44ca: ldsfld string [System.Runtime]System.String::Empty
				IL_44cf: stloc.s 208

				IL_44d1: ldloc.s 207
				IL_44d3: brfalse.s IL_44f6

				IL_44d5: ldloc.s 207
				IL_44d7: ldc.i4.s 20	<=== change this

				<---- here
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsfld<string>("Empty"),
			i => i.MatchStloc(208),
			i => i.MatchLdloc(207),
			i => i.MatchBrfalse(out _),
			i => i.MatchLdloc(207),
			i => i.MatchLdcI4(20)
		))
			throw new("server chest name edit failed");

		c.Prev.Operand = newMaxChestLength;
	}
}
