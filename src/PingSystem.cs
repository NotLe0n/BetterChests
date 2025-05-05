using System.Collections.Generic;
using System.Timers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BetterChests;

public class PingSystem : ModSystem
{
	private readonly List<Ping> pings = [];

	public int Pings => pings.Count;
	
	public void AddPing(Ping ping)
	{
		string itemTag;
		if (ping.Item.ModItem is not null) {
			itemTag = $"[i:{ping.Item.ModItem.Mod.Name}/{ping.Item.ModItem.Name}]";
		}
		else {
			itemTag = $"[i/s{ping.Item.stack},p{ping.Item.prefix}:{ping.Item.type}]";
		}

		Main.NewText($"[c/{ping.Color.Hex3()}:<{ping.PlayerName}>] pinged {itemTag}");

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
	public required Item Item { get; init; }

	private readonly Timer timer;
	
	public Chest Chest => Main.chest[ChestID];

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