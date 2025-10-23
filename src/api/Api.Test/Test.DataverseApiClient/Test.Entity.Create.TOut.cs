using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task CreateEntityAsyncWithTOut_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJson.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerCreateEntityAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerCreateEntityAsync()
            =>
            dataverseApiClient.CreateEntityAsync<StubRequestJson, StubResponseJson>(null!, TestContext.Current.CancellationToken).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.EntityCreateWithTOutInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task CreateEntityAsyncWithTOut_InputIsNotNull_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseEntityCreateIn<StubRequestJson> input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJson.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerId);

        _ = await dataverseApiClient.CreateEntityAsync<StubRequestJson, StubResponseJson>(input, TestContext.Current.CancellationToken);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public static async Task CreateEntityAsyncWithTOut_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new InvalidOperationException("Some error message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityCreateInput;
        var actual = await dataverseApiClient.CreateEntityAsync<StubRequestJson, StubResponseJson>(
            input, TestContext.Current.CancellationToken);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to create a Dataverse entity",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task CreateEntityAsyncWithTOut_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.CreateEntityAsync<StubRequestJson, StubResponseJson>(
            SomeDataverseEntityCreateInput, TestContext.Current.CancellationToken);

        Assert.StrictEqual(failure, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.StubResponseJsonOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task CreateEntityAsyncWithTOut_ResponseIsSuccess_ExpectSuccess(
        StubResponseJson? success)
    {
        var mockHttpApi = CreateMockJsonHttpApi(success.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityCreateInput;
        var actual = await dataverseApiClient.CreateEntityAsync<StubRequestJson, StubResponseJson>(
            input, TestContext.Current.CancellationToken);

        var expected = new DataverseEntityCreateOut<StubResponseJson>(success);
        Assert.StrictEqual(expected, actual);
    }
}