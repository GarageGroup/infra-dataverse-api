using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task CreateEntityAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(default(DataverseJsonResponse));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var token = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerCreateEntityAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerCreateEntityAsync()
            =>
            dataverseApiClient.CreateEntityAsync<StubRequestJson>(null!, token).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.EntityCreateInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task CreateEntityAsync_InputIsNotNull_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseEntityCreateIn<StubRequestJson> input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(default(DataverseJsonResponse));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerId);

        var token = new CancellationToken(canceled: false);
        _ = await dataverseApiClient.CreateEntityAsync(input, token);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, token), Times.Once);
    }

    [Fact]
    public static async Task CreateEntityAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new Exception("Some error message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityCreateInput;
        var actual = await dataverseApiClient.CreateEntityAsync(input, default);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to create a Dataverse entity",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task CreateEntityAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityCreateInput;
        var actual = await dataverseApiClient.CreateEntityAsync(input, default);

        Assert.StrictEqual(failure, actual);
    }

    [Fact]
    public static async Task CreateEntityAsync_ResponseIsSuccess_ExpectSuccess()
    {
        var mockHttpApi = CreateMockJsonHttpApi(default(DataverseJsonResponse));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.CreateEntityAsync(SomeDataverseEntityCreateInput, default);
        var expected = default(Unit);

        Assert.StrictEqual(expected, actual);
    }
}