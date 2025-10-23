using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task UpdateEntityAsyncWithTOut_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJson.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerUpdateEntityAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerUpdateEntityAsync()
            =>
            dataverseApiClient.UpdateEntityAsync<StubRequestJson, StubResponseJson?>(null!, TestContext.Current.CancellationToken).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.EntityUpdateWithTOutInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task UpdateEntityAsyncWithTOut_InputIsNotNull_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseEntityUpdateIn<IStubRequestJson> input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJson.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerId);

        _ = await dataverseApiClient.UpdateEntityAsync<IStubRequestJson, StubResponseJson>(
            input, TestContext.Current.CancellationToken);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public static async Task UpdateEntityAsyncWithTOut_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new Exception("Some exception message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityUpdateInput;
        var actual = await dataverseApiClient.UpdateEntityAsync<StubRequestJson, StubResponseJson>(
            input, TestContext.Current.CancellationToken);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to update a Dataverse entity",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task UpdateEntityAsyncWithTOut_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityUpdateInput;
        var actual = await dataverseApiClient.UpdateEntityAsync<StubRequestJson, StubResponseJson>(
            input, TestContext.Current.CancellationToken);

        Assert.StrictEqual(failure, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.StubResponseJsonOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task UpdateEntityAsyncWithTOut_ResponseIsSuccess_ExpectSuccess(
        StubResponseJson? success)
    {
        var mockHttpApi = CreateMockJsonHttpApi(success.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityUpdateInput;
        var actual = await dataverseApiClient.UpdateEntityAsync<StubRequestJson, StubResponseJson>(
            input, TestContext.Current.CancellationToken);

        var expected = new DataverseEntityUpdateOut<StubResponseJson>(success);
        Assert.StrictEqual(expected, actual);
    }
}