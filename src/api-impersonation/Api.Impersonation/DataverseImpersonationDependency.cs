using PrimeFuncPack;
using System;
using System.Net.Http;

namespace GarageGroup.Infra;

public static class DataverseImpersonationDependency
{
    private const string ObsoleteMessage
        =
        "This method is obsolete and will be removed in a future version.";

    public static Dependency<HttpMessageHandler> UseDataverseImpersonation(
        this Dependency<HttpMessageHandler> dependency,
        Func<IServiceProvider, IAsyncValueFunc<Unit, Guid>> callerObjectIdProviderResolver)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(callerObjectIdProviderResolver);

        return dependency.With(callerObjectIdProviderResolver).Fold<HttpMessageHandler>(ImpersonationDelegatingHandler.Create);
    }

    public static Dependency<HttpMessageHandler> UseDataverseImpersonation(
        this Dependency<HttpMessageHandler, IAsyncValueFunc<Unit, Guid>> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);

        return dependency.Fold<HttpMessageHandler>(ImpersonationDelegatingHandler.Create);
    }

    [Obsolete(ObsoleteMessage)]
    public static Dependency<HttpMessageHandler> UseDataverseImpersonation(
        this Dependency<HttpMessageHandler> dependency,
        Func<IServiceProvider, IAsyncValueFunc<Guid>> callerIdProviderResolver)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(callerIdProviderResolver);

        return dependency.With(callerIdProviderResolver).Fold<HttpMessageHandler>(LegacyImpersonationDelegatingHandler.Create);
    }

    [Obsolete(ObsoleteMessage)]
    public static Dependency<HttpMessageHandler> UseDataverseImpersonation(
        this Dependency<HttpMessageHandler, IAsyncValueFunc<Guid>> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);

        return dependency.Fold<HttpMessageHandler>(LegacyImpersonationDelegatingHandler.Create);
    }
}
