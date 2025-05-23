using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BetterChests;

public class ChestSyncingSystem : ModSystem
{
	private readonly Dictionary<int, VectorClock> ClientClocks = new(); // key: chest ID

	public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
	{
		switch (msgType) {
			case MessageID.RequestChestOpen:
				int chestID = Chest.FindChest(number, (int)number2);
				Main.NewText($"Sending ClockSyncRequest with clientID {Main.myPlayer} for chest {chestID}");
				MultiplayerSystem.GetChestClockSyncRequestPacket(chestID, Main.myPlayer).Send();
				break;
			case MessageID.SyncChestItem:
				if (Main.myPlayer == 255) break; // server sent this
				SendChestUpdate(Main.myPlayer, number, (int)number2);
				break;
		}

		return false;
	}

	private void SendChestUpdate(int whoAmI, int chestID, int slot)
	{
		Item item = Main.chest[chestID].item[slot];
		if (!ClientClocks.TryGetValue(chestID, out VectorClock? clock)) {
			clock = new VectorClock();
			ClientClocks[chestID] = clock;
		}

		clock.Increment(whoAmI);
		Main.NewText($"Clocks: [{string.Join(',', ClientClocks.Keys)}], Sent Update: {clock}");
		MultiplayerSystem.GetChestUpdatePacket(chestID, slot, item, clock).Send();
	}

	public void ReceiveChestUpdate(int chest, int slot, Item item, VectorClock incomingVC)
	{
		if (!ClientClocks.ContainsKey(chest)) {
			ClientClocks[chest] = new VectorClock();
		}

		var localVC = ClientClocks[chest];
		Main.NewText($"Clocks: [{string.Join(',', ClientClocks.Keys)}], Got Update: {localVC}");
		if (localVC.HappensBefore(incomingVC)) {
			Main.chest[chest].item[slot] = item;
			localVC.Merge(incomingVC);
		}
		else {
			Main.NewText($"Didn't happen before: local: {localVC} ;; incoming: {incomingVC} ");
		}
	}

	public void ReceiveChestClockSyncRequest(int chestID, int clientID)
	{
		Main.NewText($"ReceiveChestClockSyncRequest from {clientID}: {chestID}");
		if (Main.LocalPlayer.chest == chestID && chestID > -1) {
			Main.NewText("Send clock sync packet: " + ClientClocks[chestID]);
			MultiplayerSystem.GetChestClockSyncPacket(chestID, clientID, ClientClocks[chestID]).Send();
		}
	}

	public void ReceiveChestClockSync(int chestID, int clientID, VectorClock vc)
	{
		Main.NewText($"ReceiveChestClockSync from {clientID}: merge [{chestID}]: {vc}");
		if (!ClientClocks.TryGetValue(chestID, out var clock)) {
			ClientClocks[chestID] = vc;
		}
		else {
			clock.Merge(vc);
		}
		
		Main.NewText($"New clock for chest {chestID}: {ClientClocks[chestID]}");
	}

	public void ResetClock()
	{
		ClientClocks.Clear();
	}
}

public class ChestSyncingPlayer : ModPlayer
{
	public override void OnEnterWorld()
	{
		ModContent.GetInstance<ChestSyncingSystem>().ResetClock();
	}
}