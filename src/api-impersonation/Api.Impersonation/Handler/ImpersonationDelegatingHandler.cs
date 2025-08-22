using System;
using System.Net.Http;

namespace GarageGroup.Infra;

internal sealed partial class ImpersonationDelegatingHandler : DelegatingHandler
{
    private const string CallerIdHeaderName = "CallerObjectId";

    public static ImpersonationDelegatingHandler Create(HttpMessageHandler innerHandler, IAsyncValueFunc<Unit, Guid> callerIdProvider)
    {
        ArgumentNullException.ThrowIfNull(innerHandler);
        ArgumentNullException.ThrowIfNull(callerIdProvider);

        return new(innerHandler, callerIdProvider);
    }

    private readonly IAsyncValueFunc<Unit, Guid> callerIdProvider;

    private ImpersonationDelegatingHandler(HttpMessageHandler innerHandler, IAsyncValueFunc<Unit, Guid> callerIdProvider) : base(innerHandler)
        =>
        this.callerIdProvider = callerIdProvider;
}