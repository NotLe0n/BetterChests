using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BetterChests;

public class ChestSyncingSystem : ModSystem
{
	private static readonly Dictionary<int, VectorClock> ClientClocks = new();

	public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text,
		int number,
		float number2, float number3, float number4, int number5, int number6, int number7)
	{
		if (msgType != MessageID.SyncChestItem) return false;
		
		int clientId = Main.myPlayer;
		if (!ClientClocks.TryGetValue(clientId, out VectorClock? clock)) {
			clock = new VectorClock();
			ClientClocks[clientId] = clock;
		}

		clock.Increment(clientId);
		MultiplayerSystem.GetChestUpdatePacket(number, (byte)number2, Main.chest[number].item[(byte)number2], clock).Send();

		return false;
	}

	public static void ReceiveChestUpdate(int chest, int slot, Item item, VectorClock incomingVC, int senderId)
	{
		if (!ClientClocks.ContainsKey(Main.myPlayer)) {
			ClientClocks[Main.myPlayer] = new VectorClock();
		}

		var localVC = ClientClocks[Main.myPlayer];

		if (localVC.HappensBefore(incomingVC)) {
			Main.chest[chest].item[slot] = item;
			localVC.Merge(incomingVC);
		}
	}
}

// https://en.wikipedia.org/wiki/Vector_clock
public class VectorClock
{
	private readonly Dictionary<int, int> clock = new(); // Key: clientID, Value: Change count

	public void Increment(int clientId)
	{
		clock.TryAdd(clientId, 0);
		clock[clientId]++;
	}

	public void Merge(VectorClock other)
	{
		foreach (var kv in other.clock) {
			clock.TryAdd(kv.Key, 0);
			clock[kv.Key] = Math.Max(clock[kv.Key], kv.Value);
		}
	}

	public bool HappensBefore(VectorClock other)
	{
		bool atLeastOneLess = false;
		
		foreach (var key in clock.Keys.Union(other.clock.Keys)) {
			int localVal = clock.GetValueOrDefault(key, 0);
			int otherVal = other.clock.GetValueOrDefault(key, 0);

			if (localVal > otherVal) return false;
			if (localVal < otherVal) atLeastOneLess = true;
		}

		return atLeastOneLess;
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write(clock.Count);
		foreach (var kv in clock) {
			writer.Write(kv.Key);
			writer.Write(kv.Value);
		}
	}

	public static VectorClock Deserialize(BinaryReader reader)
	{
		var vc = new VectorClock();
		int count = reader.ReadInt32();
		for (int i = 0; i < count; i++) {
			int key = reader.ReadInt32();
			int value = reader.ReadInt32();
			vc.clock[key] = value;
		}

		return vc;
	}
}