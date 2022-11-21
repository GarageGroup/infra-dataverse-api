﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<DataverseWhoAmIOut, Failure<DataverseFailureCode>>> WhoAmIAsync(
        Unit input, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return GetCanceledAsync<DataverseWhoAmIOut>(cancellationToken);
        }

        return InnerWhoAmIAsync(cancellationToken);
    }

    private async ValueTask<Result<DataverseWhoAmIOut, Failure<DataverseFailureCode>>> InnerWhoAmIAsync(
        CancellationToken cancellationToken)
    {
        using var httpClient = CreateDataHttpClient();

        var response = await httpClient.GetAsync(WhoAmIRelativeUrl, cancellationToken).ConfigureAwait(false);
        var result = await response.ReadDataverseResultAsync<DataverseWhoAmIOutJson>(cancellationToken).ConfigureAwait(false);

        return result.MapSuccess(MapSuccess);

        static DataverseWhoAmIOut MapSuccess(DataverseWhoAmIOutJson @out)
            =>
            new(
                businessUnitId: @out.BusinessUnitId,
                userId: @out.UserId,
                organizationId: @out.OrganizationId);
    }
}