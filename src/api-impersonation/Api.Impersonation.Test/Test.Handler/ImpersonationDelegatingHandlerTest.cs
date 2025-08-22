using System;
using System.Net.Http;
using System.Threading;
using Moq;
using PrimeFuncPack;

namespace GarageGroup.Infra.Dataverse.Api.Impersonation.Test;

public static partial class ImpersonationDelegatingHandlerTest
{
    private static readonly Guid SomeCallerObjectId
        =
        new("fc293353-c3fd-4cd7-b5c8-553559d86570");

    private static HttpMessageHandler CreateImpersonationDelegatingHandler(
        HttpMessageHandler innerHandler, IAsyncValueFunc<Unit, Guid> callerIdProvider)
        =>
        Dependency.Of(
            innerHandler, callerIdProvider)
        .UseDataverseImpersonation()
        .Resolve(
            Mock.Of<IServiceProvider>());

    private static Mock<IAsyncValueFunc<Unit, Guid>> CreateMockCallerIdProvider(Guid callerObjectId)
    {
        var mock = new Mock<IAsyncValueFunc<Unit, Guid>>();

        _ = mock
            .Setup(p => p.InvokeAsync(default, It.IsAny<CancellationToken>()))
            .ReturnsAsync(callerObjectId);

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