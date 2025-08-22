using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<DataverseEntityCreateOut<TOutJson>, Failure<DataverseFailureCode>>> CreateEntityAsync<TInJson, TOutJson>(
        DataverseEntityCreateIn<TInJson> input, CancellationToken cancellationToken = default)
        where TInJson : notnull
    {
        ArgumentNullException.ThrowIfNull(input);
        return InnerCreateEntityAsync<TInJson, TOutJson>(input, cancellationToken);
    }

    private async ValueTask<Result<DataverseEntityCreateOut<TOutJson>, Failure<DataverseFailureCode>>> InnerCreateEntityAsync<TInJson, TOutJson>(
        DataverseEntityCreateIn<TInJson> input, CancellationToken cancellationToken)
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
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to create a Dataverse entity");
        }

        static DataverseEntityCreateOut<TOutJson> MapSuccess(DataverseJsonResponse response)
            =>
            new(
                value: response.Content.DeserializeOrThrow<TOutJson>());
    }

    private DataverseJsonRequest CreateDataverseJsonRequestWithRepresentation<TInJson>(IDataverseEntityCreateIn<TInJson> input)
        where TInJson : notnull
    {
        var queryParameters = new Dictionary<string, string>
        {
            ["$select"] = input.SelectFields.BuildODataParameterValue(),
            ["$expand"] = input.ExpandFields.Map(QueryParametersBuilder.BuildExpandFieldValue).BuildODataParameterValue()
        };

        var queryString = queryParameters.BuildQueryString();
        var encodedPluralName = HttpUtility.UrlEncode(input.EntityPluralName);

        return new(
            verb: DataverseHttpVerb.Post,
            url: BuildDataRequestUrl($"{encodedPluralName}{queryString}"),
            headers: GetAllHeadersWithRepresentation(input.CallerObjectId, input.SuppressDuplicateDetection).ToFlatArray(),
            content: input.EntityData.SerializeOrThrow());
    }
}