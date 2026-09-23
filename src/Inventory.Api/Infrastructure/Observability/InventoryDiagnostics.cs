using System.Diagnostics;

namespace Inventory.Api.Infrastructure.Observability;

public static class InventoryDiagnostics
{
    public const string ActivitySourceName = "AssetFlow.Inventory";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
