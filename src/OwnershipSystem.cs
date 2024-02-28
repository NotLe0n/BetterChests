using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BetterChests;

public class OwnershipSystem : ModSystem
{
	private Dictionary<int, string> ownershipMap = new();

	public Dictionary<int, string> GetMap() => ownershipMap;

	public void SetOwner(int chest, string owner)
	{
		// if the chest the current player is in is being owned by another player
		if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.chest == chest && Main.LocalPlayer.name != owner) {
			Main.LocalPlayer.chest = -1; // close chest
			UISystem.CloseChestUI();
		}
		
		ownershipMap.Add(chest, owner);
	}

	public void RemoveOwner(int chest) => ownershipMap.Remove(chest);

	public bool HasOwner(int chest) => ownershipMap.ContainsKey(chest);
	
	/// <returns>true when <paramref name="chest"/> is not owned by <paramref name="player"/></returns>
	public bool IsNotOwner(int chest, string player)
	{
		return ownershipMap.TryGetValue(chest, out string? owner) && owner != player;
	}
	
	/// <returns>true when <paramref name="chest"/> is not owned by <paramref name="player"/></returns>
	public bool IsNotOwner(int chest, string player, [NotNullWhen(true)] out string? owner)
	{
		return ownershipMap.TryGetValue(chest, out owner) && owner != player;
	}
	
	public override void SaveWorldData(TagCompound tag)
	{
		tag["chestsOwned"] = ownershipMap.Keys.ToList();
		tag["chestOwners"] = ownershipMap.Values.ToList();
	}

	public override void LoadWorldData(TagCompound tag)
	{
		var owned = tag.GetList<int>("chestsOwned");
		var owners = tag.GetList<string>("chestOwners");

		ownershipMap = owned.Zip(owners).ToDictionary(x => x.First, x => x.Second);
	}

	public override void ClearWorld()
	{
		ownershipMap.Clear();
	}
}

public class OwnershipPlayer : ModPlayer
{
	public override void OnEnterWorld()
	{
		if (Main.netMode == NetmodeID.SinglePlayer) {
			return;
		}
		
		ModPacket packet = ModContent.GetInstance<BetterChests>().GetPacket();
		packet.Write(BetterChests.GetAllOwnersPacketID);
		packet.Write(Player.whoAmI);
		packet.Send();
	}
}