using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseHttpApiTest
{
    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.ChangeSetRequestTestData), MemberType = typeof(HttpApiTestDataSource))]
    internal static async Task SendChangeSetAsync_ExpectSendAsyncCalledOnce(
        Uri dataverseUri, DataverseChangeSetRequest request, StubHttpRequestData expetedHttpRequestData)
    {
        using var response = new HttpResponseMessage
        {
            Content = new MultipartContent("mixed", $"batch_{request.BatchId}")
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response, CallbackAsync);
        var httpApi = new DataverseHttpApi(mockMessageHandler, dataverseUri);

        _ = await httpApi.SendChangeSetAsync(request, TestContext.Current.CancellationToken);

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
    public static async Task SendChangeSetAsync_ResponseIsJsonUnauthorized_ExpectUnauthorizedFailure(
        StringContent? responseContent, string failureMessage)
    {
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Unauthorized,
            Content = responseContent
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendChangeSetAsync(SomeChangeSetRequest, TestContext.Current.CancellationToken);
        var expected = Failure.Create(DataverseFailureCode.Unauthorized, failureMessage);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.FailureTestData), MemberType = typeof(HttpApiTestDataSource))]
    public static async Task SendChangeSetAsync_ResponseIsJsonFailure_ExpectFailure(
        HttpStatusCode statusCode, StringContent? responseContent, Failure<DataverseFailureCode> expected)
    {
        using var response = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = responseContent
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendChangeSetAsync(SomeChangeSetRequest, TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.UnauthorizedTestData), MemberType = typeof(HttpApiTestDataSource))]
    public static async Task SendChangeSetAsync_ResponseContainsUnauthorized_ExpectUnauthorizedFailure(
        StringContent? responseContent, string failureMessage)
    {
        using var unauthorizedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Unauthorized,
            Content = responseContent
        };

        using var response = new HttpResponseMessage
        {
            Content = new MultipartContent("mixed", "batch_f32ad236-f5a5-4d28-aa36-0051efac58ee")
            {
                new HttpMessageContent(unauthorizedResponse)
            }
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendChangeSetAsync(SomeChangeSetRequest, TestContext.Current.CancellationToken);
        var expected = Failure.Create(DataverseFailureCode.Unauthorized, failureMessage);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData]
    [InlineData("Some text")]
    public static async Task SendChangeSetAsync_ResponseIsMultiPartFailureButContentDoesNotContainFailure_ExpectFailure(
        params string[] successContents)
    {
        using var multipartContent = new MultipartContent("mixed", "batch_78132");
        foreach (var successContent in successContents)
        {
            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(successContent)
            };

            var content = new HttpMessageContent(successResponse);
            multipartContent.Add(content);
        }

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new MultipartContent("mixed", "batch_78132")
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendChangeSetAsync(SomeChangeSetRequest, TestContext.Current.CancellationToken);
        var expected = Failure.Create(DataverseFailureCode.Unknown, "An unexpected Dataverse respose status: BadRequest");

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.FailureTestData), MemberType = typeof(HttpApiTestDataSource))]
    public static async Task SendChangeSetAsync_ResponseIsMultiPartFailure_ExpectFailure(
        HttpStatusCode statusCode, StringContent? responseContent, Failure<DataverseFailureCode> expected)
    {
        using var successResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Some text")
        };

        using var failureResponse = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = responseContent
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new MultipartContent("mixed", "batch_12312")
            {
                new HttpMessageContent(successResponse),
                new HttpMessageContent(failureResponse)
            }
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendChangeSetAsync(SomeChangeSetRequest, TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.FailureTestData), MemberType = typeof(HttpApiTestDataSource))]
    public static async Task SendChangeSetAsync_ResponseContainsFailure_ExpectFailure(
        HttpStatusCode statusCode, StringContent? responseContent, Failure<DataverseFailureCode> expected)
    {
        using var successResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Some text")
        };

        using var failureResponse = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = responseContent
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new MultipartContent("mixed", "batch_12312")
            {
                new HttpMessageContent(successResponse),
                new HttpMessageContent(failureResponse)
            }
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendChangeSetAsync(SomeChangeSetRequest, TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(HttpApiTestDataSource.ChangeSetSuccessTestData), MemberType = typeof(HttpApiTestDataSource))]
    internal static async Task SendChangeSetAsync_HttpResponseIsSuccess_ExpectSuccess(
        MultipartContent responseContent, DataverseChangeSetResponse expected)
    {
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = responseContent
        };

        using var mockMessageHandler = new MockHttpMessageHandler(response);
        var httpApi = new DataverseHttpApi(mockMessageHandler, SomeDataverseBaseUri);

        var actual = await httpApi.SendChangeSetAsync(SomeChangeSetRequest, TestContext.Current.CancellationToken);
        Assert.Equal(expected, actual);
    }
}