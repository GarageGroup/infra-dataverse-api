using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task GetEntitySetAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJsonSet.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var token = new CancellationToken(canceled: false);

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerGetEntitySetAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerGetEntitySetAsync()
            =>
            dataverseApiClient.GetEntitySetAsync<StubResponseJson>(null!, token).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.EntitySetGetInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task GetEntitySetAsync_InputIsNotNull_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseEntitySetGetIn input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJsonSet.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerId);

        var token = new CancellationToken(canceled: false);
        _ = await dataverseApiClient.GetEntitySetAsync<StubResponseJson>(input, token);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, token), Times.Once);
    }

    [Fact]
    public static async Task GetEntitySetAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new InvalidOperationException("Some exception message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntitySetGetInput;
        var actual = await dataverseApiClient.GetEntitySetAsync<StubResponseJson>(input, default);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to get Dataverse entities",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task GetEntitySetAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntitySetGetInput;
        var actual = await dataverseApiClient.GetEntitySetAsync<StubResponseJson>(input, CancellationToken.None);

        Assert.StrictEqual(failure, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.StubResponseSetOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task GetEntitySetAsync_ResponseIsSuccess_ExpectSuccess(
        DataverseEntitySetJsonGetOut<StubResponseJson> success, DataverseEntitySetGetOut<StubResponseJson> expected)
    {
        var mockHttpApi = CreateMockJsonHttpApi(success.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.GetEntitySetAsync<StubResponseJson>(SomeDataverseEntitySetGetInput, CancellationToken.None);
        Assert.StrictEqual(expected, actual);
    }
}