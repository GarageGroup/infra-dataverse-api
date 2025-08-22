using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<Unit, Failure<DataverseFailureCode>>> UpdateEntityAsync<TInJson>(
        DataverseEntityUpdateIn<TInJson> input, CancellationToken cancellationToken = default)
        where TInJson : notnull
    {
        ArgumentNullException.ThrowIfNull(input);
        return InnerUpdateEntityAsync(input,cancellationToken);
    }

    private async ValueTask<Result<Unit, Failure<DataverseFailureCode>>> InnerUpdateEntityAsync<TInJson>(
        DataverseEntityUpdateIn<TInJson> input, CancellationToken cancellationToken)
        where TInJson : notnull
    {
        try
        {
            var request = CreateDataverseJsonRequest(input);
            var result = await httpApi.SendJsonAsync(request, cancellationToken).ConfigureAwait(false);

            return result.MapSuccess(Unit.From);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to update a Dataverse entity");
        }
    }

    private DataverseJsonRequest CreateDataverseJsonRequest<TInJson>(IDataverseEntityUpdateIn<TInJson> input)
        where TInJson : notnull
    {
        var encodedPluralName = HttpUtility.UrlEncode(input.EntityPluralName);
        var isUpsert = input.OperationType is DataverseUpdateOperationType.Upsert;

        return new(
            verb: DataverseHttpVerb.Patch,
            url: BuildDataRequestUrl($"{encodedPluralName}({input.EntityKey.Value})"),
            headers: GetAllHeadersWithoutRepresentation(input.CallerObjectId, input.SuppressDuplicateDetection, isUpsert).ToFlatArray(),
            content: input.EntityData.SerializeOrThrow());
    }
}