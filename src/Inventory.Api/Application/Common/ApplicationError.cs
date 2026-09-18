namespace Inventory.Api.Application.Common;

public sealed record ApplicationError(
    ApplicationErrorType Type,
    string Code,
    string Message);
