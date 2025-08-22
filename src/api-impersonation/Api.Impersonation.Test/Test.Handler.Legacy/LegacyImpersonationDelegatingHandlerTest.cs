using System;
using System.Net.Http;
using System.Threading;
using Moq;
using PrimeFuncPack;

namespace GarageGroup.Infra.Dataverse.Api.Impersonation.Test;

[Obsolete("This class is obsolete and will be removed in a future version.")]
public static partial class LegacyImpersonationDelegatingHandlerTest
{
    private static readonly Guid SomeCallerId
        =
        new("d41668b5-643f-4d30-92fd-92ce78801f51");

    private static HttpMessageHandler CreateImpersonationDelegatingHandler(
        HttpMessageHandler innerHandler, IAsyncValueFunc<Guid> callerIdProvider)
        =>
        Dependency.Of(
            innerHandler, callerIdProvider)
        .UseDataverseImpersonation()
        .Resolve(
            Mock.Of<IServiceProvider>());

    private static Mock<IAsyncValueFunc<Guid>> CreateMockCallerIdProvider(Guid callerId)
    {
        var mock = new Mock<IAsyncValueFunc<Guid>>();

        _ = mock
            .Setup(p => p.InvokeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(callerId);

        return mock;
    }

    private static Mock<IAsyncFunc<HttpRequestMessage, HttpResponseMessage>> CreateMockProxyHandler(
        HttpResponseMessage responseMessage, Action<HttpRequestMessage>? callback = default)
    {
        var mock = new Mock<IAsyncFunc<HttpRequestMessage, HttpResponseMessage>>();

        var m = mock
            .Setup(p => p.InvokeAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseMessage);

        if (callback is not null)
        {
            _ = m.Callback<HttpRequestMessage, CancellationToken>(
                (r, _) => callback.Invoke(r));
        }

        return mock;
    }
}