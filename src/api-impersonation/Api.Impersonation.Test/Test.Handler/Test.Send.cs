using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Impersonation.Test;

partial class ImpersonationDelegatingHandlerTest
{
    [Fact]
    public static async Task SendAsync_RequestIsNull_ExpectArgumentNullException()
    {
        var mockCallerIdProvider = CreateMockCallerIdProvider(SomeCallerObjectId);

        using var response = new HttpResponseMessage();
        var mockProxyHandler = CreateMockProxyHandler(response);

        using var sourceHandler = new StubHttpMessageHandler(mockProxyHandler.Object);
        var impersonationHandler = CreateImpersonationDelegatingHandler(sourceHandler, mockCallerIdProvider.Object);

        var httpClient = new HttpMessageInvoker(impersonationHandler);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => httpClient.SendAsync(null!, TestContext.Current.CancellationToken));

        Assert.Equal("request", ex.ParamName);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public static async Task SendAsync_RequestIsNotNull_ExpectCallInnerHandlerWithCallerId(
        bool isSourceRequestWithCallerId)
    {
        var callerObjectId = Guid.Parse("91930144-5ffd-4ef8-892c-83d5f428079d");
        var mockCallerIdProvider = CreateMockCallerIdProvider(callerObjectId);

        using var response = new HttpResponseMessage();
        var mockProxyHandler = CreateMockProxyHandler(response, Callback);

        using var sourceHandler = new StubHttpMessageHandler(mockProxyHandler.Object);
        var impersonationHandler = CreateImpersonationDelegatingHandler(sourceHandler, mockCallerIdProvider.Object);

        var httpClient = new HttpMessageInvoker(impersonationHandler);

        using var request = new HttpRequestMessage();
        if (isSourceRequestWithCallerId)
        {
            request.Headers.Add("CallerObjectId", "Some calleId");
        }

        _ = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);
        mockProxyHandler.Verify(p => p.InvokeAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        
        static void Callback(HttpRequestMessage actualRequest)
        {
            var actualCallerId = actualRequest.Headers.GetValues("CallerObjectId").First();
            Assert.Equal("91930144-5ffd-4ef8-892c-83d5f428079d", actualCallerId);
        }
    }

    [Fact]
    public static async Task SendAsync_RequestIsNotNull_ExpectSourceResponse()
    {
        var mockCallerIdProvider = CreateMockCallerIdProvider(SomeCallerObjectId);

        using var sourceResponse = new HttpResponseMessage();
        var mockProxyHandler = CreateMockProxyHandler(sourceResponse);

        using var sourceHandler = new StubHttpMessageHandler(mockProxyHandler.Object);
        var impersonationHandler = CreateImpersonationDelegatingHandler(sourceHandler, mockCallerIdProvider.Object);

        var httpClient = new HttpMessageInvoker(impersonationHandler);

        using var request = new HttpRequestMessage();
        var actual = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Same(sourceResponse, actual);
    }
}