using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task UpdateEntityAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(default(DataverseJsonResponse));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var token = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerUpdateEntityAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerUpdateEntityAsync()
            =>
            dataverseApiClient.UpdateEntityAsync<StubRequestJson>(null!, token).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.EntityUpdateInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task UpdateEntityAsync_InputIsNotNull_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseEntityUpdateIn<StubRequestJson> input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(default(DataverseJsonResponse));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerId);

        var token = new CancellationToken(canceled: false);
        _ = await dataverseApiClient.UpdateEntityAsync(input, token);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, token), Times.Once);
    }

    [Fact]
    public static async Task UpdateEntityAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new InvalidCastException("Some message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityUpdateInput;
        var actual = await dataverseApiClient.UpdateEntityAsync(input, default);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to update a Dataverse entity",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task UpdateEntityAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeDataverseEntityUpdateInput;
        var actual = await dataverseApiClient.UpdateEntityAsync(input, default);

        Assert.StrictEqual(failure, actual);
    }

    [Fact]
    public static async Task UpdateEntityAsync_ResponseIsSuccess_ExpectSuccess()
    {
        var mockHttpApi = CreateMockJsonHttpApi(default(DataverseJsonResponse));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.UpdateEntityAsync(SomeDataverseEntityUpdateInput, default);
        var expected = default(Unit);

        Assert.StrictEqual(expected, actual);
    }
}