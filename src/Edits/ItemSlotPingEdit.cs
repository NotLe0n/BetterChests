using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using System;

namespace BetterChests.Edits;

internal static class ItemSlotPingEdit
{
    public static void Load()
    {
        On_ItemSlot.RightClick_ItemArray_int_int += PingEdit;
        On_TileDrawing.GetTileOutlineInfo += EditOutlineCheck;
    }

    // Draw outlines around chests with pings
    private static void EditOutlineCheck(On_TileDrawing.orig_GetTileOutlineInfo orig, TileDrawing self, int x, int y, ushort typeCache, ref Color tileLight, ref Texture2D highlightTexture, ref Color highlightColor)
    {
	    var config = ModContent.GetInstance<BetterChestsConfig>();
	    orig(self, x, y, typeCache, ref tileLight, ref highlightTexture, ref highlightColor);
	    if (config.disablePings) {
		    return;
	    }

	    if (highlightTexture != null) {
		    return; // smart cursor is selecting
	    }
	    
	    var pingSystem = ModContent.GetInstance<PingSystem>();

	    foreach (var ping in pingSystem.GetPings()) {
		    var screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, 
			    (int)Main.screenPosition.X + Main.screenWidth, (int)Main.screenPosition.Y + Main.screenHeight);

		    int chestID = BetterChests.GetChest(x, y);
		    if (!screenRect.Contains(ping.Chest.x * 16, ping.Chest.y * 16) || ping.ChestID != chestID) {
			    continue;
		    }

		    highlightTexture = TextureAssets.HighlightMask[typeCache].Value;
		    highlightColor = ping.Color;
		    
		    if (Main.LocalPlayer.chest == ping.ChestID && ping.ChestID > 0) {
			    ItemSlot.SetGlow(ping.SlotIndex, Main.rgbToHsl(ping.Color).X, true);
		    }
	    }
    }

    private static void PingEdit(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
	    var config = ModContent.GetInstance<BetterChestsConfig>();
	    if (config.disablePings) {
		    orig(inv, context, slot);
		    return;
	    }
	    
	    var pingSystem = ModContent.GetInstance<PingSystem>();
	    var pingKeyPressed = ModContent.GetInstance<KeybindSystem>().ping!.Current;
	    
	    if (context == ItemSlot.Context.ChestItem && pingKeyPressed) {
		    if (inv[slot].IsAir || !Main.mouseRight || !Main.mouseRightRelease) {
			    return;
		    }
	
		    var ping = new Ping {
			    PlayerName = Main.LocalPlayer.name,
			    ChestID = Main.LocalPlayer.chest,
			    SlotIndex = slot,
			    Color = config.pingHue,
			    Item = Main.chest[Main.LocalPlayer.chest].item[slot]
		    };
		    
		    if (Main.netMode == NetmodeID.SinglePlayer) {
			    pingSystem.AddPing(ping);
		    }
		    else {
			    ping.GetPacket().Send();
		    }
	    }
        else {
            orig(inv, context, slot);
        }
    }
}