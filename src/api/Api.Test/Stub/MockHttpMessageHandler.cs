using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra.Dataverse.Api.Test;

using CallbackFunc = Func<HttpRequestMessage, Task>;

internal sealed class MockHttpMessageHandler(HttpResponseMessage response, CallbackFunc? callbackAsync = null) : HttpMessageHandler
{
    private int callsCount = 0;

    private Exception? innerException = null;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            callsCount++;

            if (callbackAsync is not null)
            {
                await callbackAsync.Invoke(request);
            }

            return response;
        }
        catch (Exception ex)
        {
            innerException = ex;
            throw;
        }
    }

    internal void Verify(int expectedTimes)
    {
        if (expectedTimes != callsCount)
        {
            throw new InvalidOperationException(
                $"HttpMessageHandler was expected to be called {expectedTimes} times but it was {callsCount}");
        }

        if (innerException is not null)
        {
            throw innerException;
        }
    }
}