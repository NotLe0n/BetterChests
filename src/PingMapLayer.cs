using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests;

public class PingMapLayer : ModMapLayer
{
	public override void Draw(ref MapOverlayDrawContext context, ref string text)
	{
		var pingSystem = ModContent.GetInstance<PingSystem>();
		var config = ModContent.GetInstance<BetterChestsConfig>();
		if (pingSystem.Pings == 0 || config.disablePings) {
			return;
		}
		
		for (int i = 0; i < pingSystem.Pings; i++) {
			Ping ping = pingSystem.GetPings()[i];
			Texture2D itemTexture = TextureAssets.Item[ping.Item.type].Value;
			
			var sameChestPings = pingSystem.GetPings().Where(x => x.ChestID == ping.ChestID).ToList();
			int num = sameChestPings.Count;
			int indexInGroup = sameChestPings.IndexOf(ping);

			// Grid dimensions
			int gridCols = (int)Math.Ceiling(Math.Sqrt(num));
			int gridRows = (int)Math.Ceiling(num / (float)gridCols);

			int col = indexInGroup % gridCols;
			int row = indexInGroup / gridCols;

			// Calculate offset from center
			// Configurable padding (in tile-space units)
			const float Padding = 0.6f;

			// Centered offset calculation
			float totalWidth = (gridCols - 1) * Padding;
			float totalHeight = (gridRows - 1) * Padding;
			float offsetX = (col * Padding) - (totalWidth / 2f);
			float offsetY = (row * Padding) - (totalHeight / 2f);

			
			Vector2 chestPos = new Vector2(ping.Chest.x + 1 + offsetX, ping.Chest.y + 1 + offsetY);
			if (TileID.Sets.BasicDresser[Main.tile[ping.Chest.x, ping.Chest.y].TileType]) {
				chestPos.X += 0.5f; // shift horizontally to center on 3-wide dresser
			}
			
			int frameCount = 1;
			Rectangle itemFrameRect = itemTexture.Frame();
			if (Main.itemAnimations[ping.Item.type] != null) {
				itemFrameRect = Main.itemAnimations[ping.Item.type].GetFrame(itemTexture);
				frameCount = Main.itemAnimations[ping.Item.type].FrameCount;
			}
			
			const int MaxSize = 20;
			float drawScale = 1f;
			if (itemTexture.Width > MaxSize || itemTexture.Height / frameCount > MaxSize) {
				drawScale = MaxSize / (float)(itemFrameRect.Width <= itemFrameRect.Height ?
					itemFrameRect.Height :
					itemFrameRect.Width);
			}
			
			if (context.Draw(itemTexture, chestPos, Color.White, new SpriteFrame(1, 1), drawScale, drawScale*1.5f, Alignment.Center).IsMouseOver) {
				text = ping.Item.HoverName;
			}
		}
	}
}