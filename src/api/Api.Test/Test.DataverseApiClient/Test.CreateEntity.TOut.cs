using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GGroupp.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task CreateEntityAsyncWithTOut_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockHttpApi<StubRequestJson, StubResponseJson>(SomeResponseJson);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object);

        var token = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerCreateEntityAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerCreateEntityAsync()
            =>
            dataverseApiClient.CreateEntityAsync<StubRequestJson, StubResponseJson>(null!, token).AsTask();
    }

    [Fact]
    public static void CreateEntityAsyncWithTOut_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        var mockHttpApi = CreateMockHttpApi<StubRequestJson, Unit>(default(Unit));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object);

        var token = new CancellationToken(canceled: true);

        var actualTask = dataverseApiClient.CreateEntityAsync<StubRequestJson, Unit>(
            SomeDataverseEntityCreateInput, token);

        Assert.True(actualTask.IsCanceled);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.EntityCreateWithTOutInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task CreateEntityAsyncWithTOut_CancellationTokenIsNotCanceled_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseEntityCreateIn<StubRequestJson> input, DataverseHttpRequest<StubRequestJson> expectedRequest)
    {
        var mockHttpApi = CreateMockHttpApi<StubRequestJson, StubResponseJson>(SomeResponseJson);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, callerId);

        var token = new CancellationToken(canceled: false);
        _ = await dataverseApiClient.CreateEntityAsync<StubRequestJson, StubResponseJson>(input, token);

        mockHttpApi.Verify(p => p.InvokeAsync<StubRequestJson, StubResponseJson>(expectedRequest, token), Times.Once);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task CreateEntityAsyncWithTOut_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockHttpApi<StubRequestJson, StubResponseJson>(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object);

        var actual = await dataverseApiClient.CreateEntityAsync<StubRequestJson, StubResponseJson>(
            SomeDataverseEntityCreateInput, CancellationToken.None);

        Assert.StrictEqual(failure, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.StubResponseJsonOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task CreateEntityAsyncWithTOut_ResponseIsSuccess_ExpectSuccess(
        StubResponseJson? success)
    {
        var mockHttpApi = CreateMockHttpApi<StubRequestJson, StubResponseJson>(success);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object);

        var input = SomeDataverseEntityCreateInput;
        var actual = await dataverseApiClient.CreateEntityAsync<StubRequestJson, StubResponseJson>(input, default);

        var expected = new DataverseEntityCreateOut<StubResponseJson>(success);
        Assert.StrictEqual(expected, actual);
    }
}