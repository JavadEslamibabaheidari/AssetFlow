namespace Inventory.Api.Application.Common;

public sealed class ApplicationResult<T>
{
    private ApplicationResult(T? value, ApplicationError? error)
    {
        Value = value;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public T? Value { get; }

    public ApplicationError? Error { get; }

    public static ApplicationResult<T> Success(T value) => new(value, null);

    public static ApplicationResult<T> Validation(string code, string message) =>
        Failure(ApplicationErrorType.Validation, code, message);

    public static ApplicationResult<T> NotFound(string code, string message) =>
        Failure(ApplicationErrorType.NotFound, code, message);

    public static ApplicationResult<T> Conflict(string code, string message) =>
        Failure(ApplicationErrorType.Conflict, code, message);

    private static ApplicationResult<T> Failure(ApplicationErrorType type, string code, string message) =>
        new(default, new ApplicationError(type, code, message));
}
