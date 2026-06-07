using ArchiSteamFarm.Core;
using ArchiSteamFarm.NLog;
using ArchiSteamFarm.Steam.Data;
using SteamKit2.Internal;
using SteamKit2;
using System.Threading.Tasks;
using System;

namespace ItemRemover.Handlers;

internal sealed class InventoryHandler : ClientMsgHandler {
	private readonly ArchiLogger ArchiLogger;
	private readonly Inventory UnifiedInventoryService;

	internal InventoryHandler(ArchiLogger archiLogger, SteamUnifiedMessages steamUnifiedMessages) {
		ArgumentNullException.ThrowIfNull(archiLogger);
		ArgumentNullException.ThrowIfNull(steamUnifiedMessages);

		ArchiLogger = archiLogger;
		UnifiedInventoryService = steamUnifiedMessages.CreateService<Inventory>();
	}

	public override void HandleMsg(IPacketMsg packetMsg) => ArgumentNullException.ThrowIfNull(packetMsg);

	internal async Task<SteamUnifiedMessages.ServiceMethodResponse<CInventory_Response>?> ConsumeItem(uint appID, Asset asset, ulong steamID) {
		if (Client == null) {
			throw new InvalidOperationException(nameof(Client));
		}

		if (!Client.IsConnected) {
			return null;
		}

		CInventory_ConsumeItem_Request request = new() {
			appid = appID,
			itemid = asset.AssetID,
			quantity = asset.Amount,
			steamid = steamID
		};

		SteamUnifiedMessages.ServiceMethodResponse<CInventory_Response> response;

		try {
			response = await UnifiedInventoryService.ConsumeItem(request).ToLongRunningTask().ConfigureAwait(false);
		} catch (Exception e) {
			ArchiLogger.LogGenericWarningException(e);

			return null;
		}

		return response;
	}
}
