using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task DeleteEntityAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(default(DataverseJsonResponse));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var token = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerDeleteEntityAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerDeleteEntityAsync()
            =>
            dataverseApiClient.DeleteEntityAsync(null!, token).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.EntityDeleteInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task DeleteEntityAsync_InputIsNotNull_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseEntityDeleteIn input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJson.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerId);

        var token = new CancellationToken(canceled: false);
        _ = await dataverseApiClient.DeleteEntityAsync(input, token);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, token), Times.Once);
    }

    [Fact]
    public static async Task DeleteEntityAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new InvalidOperationException("Some Exception message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityDeleteInput;
        var actual = await dataverseApiClient.DeleteEntityAsync(input, default);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to delete a Dataverse entity",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task DeleteEntityAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.DeleteEntityAsync(SomeDataverseEntityDeleteInput, default);
        Assert.StrictEqual(failure, actual);
    }

    [Fact]
    public static async Task DeleteEntityAsync_ResponseIsSuccess_ExpectSuccess()
    {
        var mockHttpApi = CreateMockJsonHttpApi(default(DataverseJsonResponse));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.DeleteEntityAsync(SomeDataverseEntityDeleteInput, default);
        var expected = Result.Success<Unit>(default);

        Assert.StrictEqual(expected, actual);
    }
}