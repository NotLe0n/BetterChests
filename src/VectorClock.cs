using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;

namespace BetterChests;

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

			Main.NewText($"local: {localVal}, other: {otherVal}");
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

	public override string ToString()
	{
		string str = "";
		foreach (var (key, value) in clock) {
			str += $"{{{key}: {value}}}, ";
		}

		return str;
	}
}