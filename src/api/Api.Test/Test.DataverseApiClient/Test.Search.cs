using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task SearchAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeSearchJsonOut.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerSearchAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerSearchAsync()
            =>
            dataverseApiClient.SearchAsync(null!, TestContext.Current.CancellationToken).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.SearchInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task SearchAsync_InputIsNotNull_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseSearchIn input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeSearchJsonOut.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerId);

        _ = await dataverseApiClient.SearchAsync(input, TestContext.Current.CancellationToken);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public static async Task SearchAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new InvalidOperationException("Some Exception message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.SearchAsync(SomeDataverseSearchInput, TestContext.Current.CancellationToken);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to search Dataverse entities",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task SearchAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.SearchAsync(SomeDataverseSearchInput, TestContext.Current.CancellationToken);
        Assert.StrictEqual(failure, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.SearchOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task SearchAsync_ResponseIsSuccess_ExpectSuccess(
        DataverseSearchJsonOut success, DataverseSearchOut expected)
    {
        var mockHttpApi = CreateMockJsonHttpApi(success.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.SearchAsync(SomeDataverseSearchInput, TestContext.Current.CancellationToken);
        Assert.StrictEqual(expected, actual);
    }
}