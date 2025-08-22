using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task FetchXmlAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeResponseJsonSet.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var token = new CancellationToken(canceled: false);

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerGetEntitySetAsync);
        Assert.Equal("input", ex.ParamName);

        Task InnerGetEntitySetAsync()
            =>
            dataverseApiClient.FetchXmlAsync<StubResponseJson>(null!, token).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FetchXmlInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task FetchXmlAsync_InputIsNotNull_ExpectHttpRequestCalledOnce(
        Guid? callerUserId, DataverseFetchXmlIn input, DataverseJsonRequest expectedRequest)
    {
        var fixture = new Fixture();
        var unCustomizedFixture = new Fixture();
        
        var customization = new SupportMutableValueTypesCustomization();
        customization.Customize(fixture);
        fixture.Register(() => $"<cookie pagenumber='2' pagingcookie='{unCustomizedFixture.Create<string>()}'/>");

        var success = fixture.Create<DataverseFetchXmlOutJson<StubResponseJson>>().InnerToJsonResponse();

        var mockHttpApi = CreateMockJsonHttpApi(success);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerUserId);

        var token = new CancellationToken(canceled: false);
        _ = await dataverseApiClient.FetchXmlAsync<StubResponseJson>(input, token);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, token), Times.Once);
    }

    [Fact]
    public static async Task FetchXmlAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new Exception("Some text of error");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = new Fixture().Create<DataverseFetchXmlIn>();
        var actual = await dataverseApiClient.FetchXmlAsync<StubResponseJson>(input, default);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to fetch Dataverse entities",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task FetchXmlAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());
        
        var input = new Fixture().Create<DataverseFetchXmlIn>();
        var actual = await dataverseApiClient.FetchXmlAsync<StubResponseJson>(input, CancellationToken.None);

        Assert.StrictEqual(failure, actual);
    }
    
    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FetchStubResponseJsonTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task FetchXmlAsync_ResponseIsSuccess_ExpectSuccess(
        DataverseFetchXmlOutJson<StubResponseJson> success, DataverseFetchXmlOut<StubResponseJson> expected)
    {
        var mockHttpApi = CreateMockJsonHttpApi(success.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = new Fixture().Create<DataverseFetchXmlIn>();
        var actual = await dataverseApiClient.FetchXmlAsync<StubResponseJson>(input, CancellationToken.None);
        
        Assert.StrictEqual(expected, actual);
    }
}