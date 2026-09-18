namespace Inventory.Api.Domain.Entities;

public sealed class SalesChannel
{
    private SalesChannel()
    {
    }

    public SalesChannel(Guid id, string code, string name, DateTimeOffset createdAtUtc)
    {
        Id = id;
        Code = code;
        Name = name;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }
}
