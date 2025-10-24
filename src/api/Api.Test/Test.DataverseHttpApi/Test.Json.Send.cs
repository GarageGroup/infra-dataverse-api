using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseHttpApiTest
{
    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.JsonRequestTestData), MemberType = typeof(HttpApiTestDataSource))]
    internal static async Task SendJsonAsync_ExpectSendAsyncCalledOnce(
        Uri dataverseUri, DataverseJsonRequest request, StubHttpRequestData expetedHttpRequestData)
    {
        using var response = new HttpResponseMessage();

        using var mockMessageHandler = new MockHttpMessageHandler(response, CallbackAsync);
        var httpApi = new DataverseHttpApi(mockMessageHandler, dataverseUri);

        _ = await httpApi.SendJsonAsync(request, TestContext.Current.CancellationToken);

        mockMessageHandler.Verify(1);

        async Task CallbackAsync(HttpRequestMessage requestMessage)
        {
            Assert.Equal(expetedHttpRequestData.Method, requestMessage.Method);

            var actualRequestUrl = requestMessage.RequestUri?.ToString();
            Assert.Equal(expetedHttpRequestData.RequestUrl, actualRequestUrl, ignoreCase: true);

            var actualContent = await requestMessage.ReadStringContentAsync();
            Assert.Equal(expetedHttpRequestData.Content, actualContent);

            var actualHeaderValues = ReadHeaderValues(requestMessage);
            Assert.Equal(expetedHttpRequestData.Headers.AsEnumerable(), actualHeaderValues.AsEnumerable());
        }
    }

    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.UnauthorizedTestData), MemberType = typeof(HttpApiTestDataSource))]
    public static async Task SendJsonAsync_ResponseIsUnauthorized_ExpectUnauthorizedFailure(
        StringContent? responseContent, string failureMessage)
    {
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Unauthorized,
            Content = responseContent
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendJsonAsync(SomeJsonRequest, TestContext.Current.CancellationToken);
        var expected = Failure.Create(DataverseFailureCode.Unauthorized, failureMessage);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.FailureTestData), MemberType = typeof(HttpApiTestDataSource))]
    public static async Task SendJsonAsync_ResponseIsFailure_ExpectFailure(
        HttpStatusCode statusCode, StringContent? responseContent, Failure<DataverseFailureCode> expected)
    {
        using var response = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = responseContent
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendJsonAsync(SomeJsonRequest, TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.UnitJsonSuccessTestData), MemberType = typeof(HttpApiTestDataSource))]
    public static async Task SendJsonAsync_HttpResponseIsSuccessNoContent_ExpectSuccessNullContent(
        StringContent? responseContent)
    {
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NoContent,
            Content = responseContent
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendJsonAsync(SomeJsonRequest, TestContext.Current.CancellationToken);
        var expected = default(DataverseJsonResponse);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.JsonSuccessTestData), MemberType = typeof(HttpApiTestDataSource))]
    internal static async Task SendJsonAsync_HttpResponseIsSuccess_ExpectSuccess(
        StringContent? responseContent, DataverseJsonResponse expected)
    {
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = responseContent
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendJsonAsync(SomeJsonRequest, TestContext.Current.CancellationToken);
        Assert.Equal(expected, actual);
    }
}