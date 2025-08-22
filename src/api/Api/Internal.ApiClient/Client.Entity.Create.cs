using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<Unit, Failure<DataverseFailureCode>>> CreateEntityAsync<TInJson>(
        DataverseEntityCreateIn<TInJson> input, CancellationToken cancellationToken = default)
        where TInJson : notnull
    {
        ArgumentNullException.ThrowIfNull(input);
        return InnerCreateEntityAsync(input, cancellationToken);
    }

    private async ValueTask<Result<Unit, Failure<DataverseFailureCode>>> InnerCreateEntityAsync<TInJson>(
        DataverseEntityCreateIn<TInJson> input, CancellationToken cancellationToken)
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
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to create a Dataverse entity");
        }
    }

    private DataverseJsonRequest CreateDataverseJsonRequest<TInJson>(IDataverseEntityCreateIn<TInJson> input)
        where TInJson : notnull
    {
        var encodedPluralName = HttpUtility.UrlEncode(input.EntityPluralName);

        return new(
            verb: DataverseHttpVerb.Post,
            url: BuildDataRequestUrl(encodedPluralName),
            headers: GetAllHeadersWithoutRepresentation(input.CallerObjectId, input.SuppressDuplicateDetection).ToFlatArray(),
            content: input.EntityData.SerializeOrThrow());
    }
}