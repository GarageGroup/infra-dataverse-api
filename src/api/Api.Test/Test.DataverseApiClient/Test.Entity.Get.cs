using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task GetEntityAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJson.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerGetEntityAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerGetEntityAsync()
            =>
            dataverseApiClient.GetEntityAsync<StubResponseJson>(null!, TestContext.Current.CancellationToken).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.EntityGetInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task GetEntityAsync_InputIsNotNull_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseEntityGetIn input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJson.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerId);

        _ = await dataverseApiClient.GetEntityAsync<StubResponseJson>(input, TestContext.Current.CancellationToken);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public static async Task GetEntityAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new Exception("Some exception message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityGetInput;
        var actual = await dataverseApiClient.GetEntityAsync<StubResponseJson>(input, TestContext.Current.CancellationToken);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to get a Dataverse entity",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task GetEntityAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.GetEntityAsync<StubResponseJson>(
            SomeDataverseEntityGetInput, TestContext.Current.CancellationToken);

        Assert.StrictEqual(failure, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.StubResponseJsonOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task GetEntityAsync_ResponseIsSuccess_ExpectSuccess(
        StubResponseJson? success)
    {
        var mockHttpApi = CreateMockJsonHttpApi(success.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.GetEntityAsync<StubResponseJson>(
            SomeDataverseEntityGetInput, TestContext.Current.CancellationToken);

        var expected = new DataverseEntityGetOut<StubResponseJson>(success);

        Assert.StrictEqual(expected, actual);
    }
}