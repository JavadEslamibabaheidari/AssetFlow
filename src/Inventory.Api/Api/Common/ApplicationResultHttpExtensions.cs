using Inventory.Api.Application.Common;

namespace Inventory.Api.Api.Common;

public static class ApplicationResultHttpExtensions
{
    public static IResult ToHttpResult<T>(
        this ApplicationResult<T> result,
        Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return onSuccess(result.Value);
        }

        return result.Error?.ToProblemHttpResult()
            ?? Results.Problem(
                title: "Unexpected application result.",
                statusCode: StatusCodes.Status500InternalServerError);
    }

    private static IResult ToProblemHttpResult(this ApplicationError error)
    {
        var statusCode = error.Type switch
        {
            ApplicationErrorType.Validation => StatusCodes.Status400BadRequest,
            ApplicationErrorType.NotFound => StatusCodes.Status404NotFound,
            ApplicationErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(
            title: error.Message,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>
            {
                ["code"] = error.Code
            });
    }
}
