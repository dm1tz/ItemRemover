using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam.Data;
using ArchiSteamFarm.Steam;
using PluginLocale = ItemRemover.Localization;
using SteamKit2.Internal;
using SteamKit2;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ItemRemover.Handlers;

internal static class RemovalHandler {
	private static byte RemovalLimiterDelay => ItemRemoverPlugin.Config?.RemovalLimiterDelay ?? ItemRemoverConfig.DefaultRemovalLimiterDelay;
	private static readonly SemaphoreSlim RemovalSemaphore = new(1, 1);
	internal static async Task<string> RemoveInventory(Bot bot, uint appID, ulong contextID, Func<Asset, bool>? filterFunction = null) {
		ArgumentNullException.ThrowIfNull(bot);

		InventoryHandler? inventoryHandler = bot.GetHandler<InventoryHandler>();

		if (inventoryHandler == null) {
			throw new InvalidOperationException(nameof(inventoryHandler));
		}

		await RemovalSemaphore.WaitAsync().ConfigureAwait(false);

		try {

			filterFunction ??= static _ => true;

			HashSet<Asset> inventory = [];

			try {
				inventory = await bot.ArchiHandler.GetMyInventoryAsync(appID, contextID).Where(item => filterFunction(item)).ToHashSetAsync().ConfigureAwait(false);
			} catch (TimeoutException e) {
				bot.ArchiLogger.LogGenericWarningException(e);
			} catch (Exception e) {
				bot.ArchiLogger.LogGenericException(e);
			}

			if (inventory.Count == 0) {
				return string.Format(CultureInfo.CurrentCulture, Strings.ErrorIsEmpty, nameof(inventory));
			}

			uint removalCount = 0;

			foreach (Asset asset in inventory) {
				SteamUnifiedMessages.ServiceMethodResponse<CInventory_Response>? response = await inventoryHandler.ConsumeItem(appID, asset, bot.SteamID).ConfigureAwait(false);

				if (response == null) {
					return string.Format(CultureInfo.CurrentCulture, Strings.ErrorIsEmpty, nameof(inventory));
				}

				if (response.Result != EResult.OK) {
					return string.Format(CultureInfo.CurrentCulture, Strings.WarningFailedWithError, response.Result);
				}

				removalCount++;

				await Task.Delay(RemovalLimiterDelay * 1000).ConfigureAwait(false);
			}

			return PluginLocale.Strings.FormatBotDoneRemoving(removalCount);
		} finally {
			_ = RemovalSemaphore.Release();
		}
	}
}
