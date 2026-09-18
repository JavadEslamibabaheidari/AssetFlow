using Inventory.Api.Application.Common;

namespace Inventory.Api.Tests;

public sealed class ApplicationResultTests
{
    [Fact]
    public void Success_CapturesValueWithoutError()
    {
        var result = ApplicationResult<string>.Success("created");

        Assert.True(result.IsSuccess);
        Assert.Equal("created", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Conflict_CapturesExpectedError()
    {
        var result = ApplicationResult<string>.Conflict("vendor.duplicateName", "Vendor name already exists.");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.NotNull(result.Error);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
        Assert.Equal("vendor.duplicateName", result.Error.Code);
        Assert.Equal("Vendor name already exists.", result.Error.Message);
    }
}
