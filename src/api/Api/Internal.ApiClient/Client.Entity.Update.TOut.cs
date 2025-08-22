using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<DataverseEntityUpdateOut<TOutJson>, Failure<DataverseFailureCode>>> UpdateEntityAsync<TInJson, TOutJson>(
        DataverseEntityUpdateIn<TInJson> input, CancellationToken cancellationToken = default)
        where TInJson : notnull
    {
        ArgumentNullException.ThrowIfNull(input);
        return InnerUpdateEntityAsync<TInJson, TOutJson>(input, cancellationToken);
    }

    private async ValueTask<Result<DataverseEntityUpdateOut<TOutJson>, Failure<DataverseFailureCode>>> InnerUpdateEntityAsync<TInJson, TOutJson>(
        DataverseEntityUpdateIn<TInJson> input, CancellationToken cancellationToken)
        where TInJson : notnull
    {
        try
        {
            var request = CreateDataverseJsonRequestWithRepresentation(input);
            var result = await httpApi.SendJsonAsync(request, cancellationToken).ConfigureAwait(false);

            return result.MapSuccess(MapSuccess);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to update a Dataverse entity");
        }

        static DataverseEntityUpdateOut<TOutJson> MapSuccess(DataverseJsonResponse response)
            =>
            new(
                value: response.Content.DeserializeOrThrow<TOutJson>());
    }

    private DataverseJsonRequest CreateDataverseJsonRequestWithRepresentation<TInJson>(IDataverseEntityUpdateIn<TInJson> input)
        where TInJson : notnull
    {
        var queryParameters = new Dictionary<string, string>
        {
            ["$select"] = input.SelectFields.BuildODataParameterValue(),
            ["$expand"] = input.ExpandFields.Map(QueryParametersBuilder.BuildExpandFieldValue).BuildODataParameterValue(),
        };

        var queryString = queryParameters.BuildQueryString();
        var encodedPluralName = HttpUtility.UrlEncode(input.EntityPluralName);
        var isUpsert = input.OperationType is DataverseUpdateOperationType.Upsert;

        return new(
            verb: DataverseHttpVerb.Patch,
            url: BuildDataRequestUrl($"{encodedPluralName}({input.EntityKey.Value}){queryString}"),
            headers: GetAllHeadersWithRepresentation(input.CallerObjectId, input.SuppressDuplicateDetection, isUpsert).ToFlatArray(),
            content: input.EntityData.SerializeOrThrow());
    }
}