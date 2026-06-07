using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ItemRemover;

[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "The class is used during json deserialization")]
internal sealed class ItemRemoverConfig {
	internal const byte DefaultRemovalLimiterDelay = 1;

	[JsonInclude]
	internal byte RemovalLimiterDelay { get; private init; } = DefaultRemovalLimiterDelay;

	[JsonConstructor]
	private ItemRemoverConfig() { }
}
