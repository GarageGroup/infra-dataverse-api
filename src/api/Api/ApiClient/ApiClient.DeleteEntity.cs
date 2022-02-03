using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<Unit, Failure<DataverseFailureCode>>> DeleteEntityAsync(
        DataverseEntityDeleteIn input, CancellationToken cancellationToken = default)
    {
        _ = input ?? throw new ArgumentNullException(nameof(input));

        if (cancellationToken.IsCancellationRequested)
        {
            return GetCanceledAsync<Unit>(cancellationToken);
        }

        return InnerDeleteEntityAsync(input, cancellationToken);
    }

    private async ValueTask<Result<Unit, Failure<DataverseFailureCode>>> InnerDeleteEntityAsync(
        DataverseEntityDeleteIn input, CancellationToken cancellationToken)
    {
        using var httpClient = await DataverseHttpHelper.InternalCreateHttpClientAsync(
                messageHandler,
                configuration,
                apiVersion: ApiVersionData,
                apiType: ApiTypeData)
            .ConfigureAwait(false);

        var entitiyDeleteUrl = $"{input.EntityPluralName}({input.EntityKey.Value})";

        var response = await httpClient.DeleteAsync(entitiyDeleteUrl, cancellationToken).ConfigureAwait(false);
        return await response.InternalReadDataverseResultAsync<Unit>(cancellationToken).ConfigureAwait(false);
    }  
}