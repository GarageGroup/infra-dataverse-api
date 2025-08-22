using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra;

partial class ImpersonationDelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Headers.Contains(CallerIdHeaderName))
        {
            request.Headers.Remove(CallerIdHeaderName);
        }

        var callerId = await callerIdProvider.InvokeAsync(default, cancellationToken).ConfigureAwait(false);
        request.Headers.Add(CallerIdHeaderName, callerId.ToString("D", CultureInfo.InvariantCulture));

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}