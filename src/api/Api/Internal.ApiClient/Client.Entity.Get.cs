using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<DataverseEntityGetOut<TJson>, Failure<DataverseFailureCode>>> GetEntityAsync<TJson>(
        DataverseEntityGetIn input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        return InnerGetEntityAsync<TJson>(input, cancellationToken);
    }

    private async ValueTask<Result<DataverseEntityGetOut<TJson>, Failure<DataverseFailureCode>>> InnerGetEntityAsync<TJson>(
        DataverseEntityGetIn input, CancellationToken cancellationToken)
    {
        try
        {
            var queryParameters = new Dictionary<string, string>
            {
                ["$select"] = input.SelectFields.BuildODataParameterValue(),
                ["$expand"] = input.ExpandFields.Map(QueryParametersBuilder.BuildExpandFieldValue).BuildODataParameterValue()
            };

            var queryString = queryParameters.BuildQueryString();
            var encodedPluralName = HttpUtility.UrlEncode(input.EntityPluralName);

            var request = new DataverseJsonRequest(
                verb: DataverseHttpVerb.Get,
                url: BuildDataRequestUrl($"{encodedPluralName}({input.EntityKey.Value}){queryString}"),
                headers: GetAllHeaders(input.CallerObjectId, BuildPreferHeader(input.IncludeAnnotations)).ToFlatArray(),
                content: default);

            var result = await httpApi.SendJsonAsync(request, cancellationToken).ConfigureAwait(false);
            return result.MapSuccess(MapSuccess);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to get a Dataverse entity");
        }

        static DataverseEntityGetOut<TJson> MapSuccess(DataverseJsonResponse response)
            =>
            new(
                value: response.Content.DeserializeOrThrow<TJson>());
    }
}