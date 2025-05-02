using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests;

public class PingSystem : ModSystem
{
	private readonly List<Ping> pings = [];

	public int Pings => pings.Count;
	
	public void AddPing(Ping ping)
	{
		var inv = Main.chest[ping.ChestID].item;
		string itemTag;
		if (inv[ping.SlotIndex].ModItem is not null) {
			itemTag = $"[i:{inv[ping.SlotIndex].ModItem.Mod.Name}/{inv[ping.SlotIndex].ModItem.Name}]";
		}
		else {
			itemTag = $"[i/s{inv[ping.SlotIndex].stack},p{inv[ping.SlotIndex].prefix}:{inv[ping.SlotIndex].type}]";
		}

		Main.NewText($"[c/{ping.Color.Hex3()}:<{ping.PlayerName}>] pinged {itemTag}");
		if (Main.LocalPlayer.chest == ping.ChestID && ping.ChestID > 0) {
			ItemSlot.SetGlow(ping.SlotIndex, Main.rgbToHsl(ping.Color).X, true);
		}

		pings.Add(ping);
	}

	public void RemovePing(Ping ping)
	{
		pings.Remove(ping);
	}
	
	public IReadOnlyList<Ping> GetPings()
	{
		return pings;
	}
}

public class Ping
{
	public int ChestID { get; init; }
	public int SlotIndex { get; init; }
	public required string PlayerName { get; init; }
	public required Color Color { get; init; }

	private readonly Timer timer;
	
	public Chest Chest => Main.chest[ChestID];
	public Item Item => Main.chest[ChestID].item[SlotIndex];

	public Ping()
	{
		timer = new Timer {
			Enabled = true,
			AutoReset = false,
			Interval = ModContent.GetInstance<BetterChestsConfig>().pingTime * 1000
		};
		
		timer.Elapsed += (sender, e) =>
		{
			ModContent.GetInstance<PingSystem>().RemovePing(this);
		};
	}
}