using System;
using System.Net.Http;

namespace GarageGroup.Infra;

internal sealed partial class LegacyImpersonationDelegatingHandler : DelegatingHandler
{
    private const string CallerIdHeaderName = "MSCRMCallerID";

    public static LegacyImpersonationDelegatingHandler Create(HttpMessageHandler innerHandler, IAsyncValueFunc<Guid> callerIdProvider)
    {
        ArgumentNullException.ThrowIfNull(innerHandler);
        ArgumentNullException.ThrowIfNull(callerIdProvider);

        return new(innerHandler, callerIdProvider);
    }

    private readonly IAsyncValueFunc<Guid> callerIdProvider;

    private LegacyImpersonationDelegatingHandler(HttpMessageHandler innerHandler, IAsyncValueFunc<Guid> callerIdProvider) : base(innerHandler)
        =>
        this.callerIdProvider = callerIdProvider;
}