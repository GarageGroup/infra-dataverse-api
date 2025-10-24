using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Impersonation.Test;

partial class LegacyImpersonationDelegatingHandlerTest
{
    [Fact]
    public static async Task SendAsync_RequestIsNull_ExpectArgumentNullException()
    {
        var mockCallerIdProvider = CreateMockCallerIdProvider(SomeCallerId);

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
        var mockCallerIdProvider = CreateMockCallerIdProvider(new("ac3a51a1-c8e1-4848-a556-ca75935d9e8c"));

        using var response = new HttpResponseMessage();
        var mockProxyHandler = CreateMockProxyHandler(response, Callback);

        using var sourceHandler = new StubHttpMessageHandler(mockProxyHandler.Object);
        var impersonationHandler = CreateImpersonationDelegatingHandler(sourceHandler, mockCallerIdProvider.Object);

        var httpClient = new HttpMessageInvoker(impersonationHandler);

        using var request = new HttpRequestMessage();
        if (isSourceRequestWithCallerId)
        {
            request.Headers.Add("MSCRMCallerID", "Some calleId");
        }

        _ = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);
        mockProxyHandler.Verify(p => p.InvokeAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        
        static void Callback(HttpRequestMessage actualRequest)
        {
            var actualCallerId = actualRequest.Headers.GetValues("MSCRMCallerID").First();
            Assert.Equal("ac3a51a1-c8e1-4848-a556-ca75935d9e8c", actualCallerId);
        }
    }

    [Fact]
    public static async Task SendAsync_RequestIsNotNull_ExpectSourceResponse()
    {
        var mockCallerIdProvider = CreateMockCallerIdProvider(SomeCallerId);

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