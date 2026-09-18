namespace Inventory.Api.Application.Vendors;

public sealed record VendorDto(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAtUtc);
