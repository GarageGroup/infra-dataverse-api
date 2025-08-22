using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<DataverseSearchOut, Failure<DataverseFailureCode>>> SearchAsync(
        DataverseSearchIn input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        return InnerSearchAsync(input, cancellationToken);
    }

    private async ValueTask<Result<DataverseSearchOut, Failure<DataverseFailureCode>>> InnerSearchAsync(
        DataverseSearchIn input, CancellationToken cancellationToken)
    {
        try
        {
            var searchIn = new DataverseSearchJsonIn
            {
                Search = input.Search,
                Entities = input.Entities?.FilterNotEmpty(),
                Facets = input.Facets?.FilterNotEmpty(),
                Filter = input.Filter,
                ReturnTotalRecordCount = input.ReturnTotalRecordCount,
                Skip = input.Skip,
                Top = input.Top,
                OrderBy = input.OrderBy?.FilterNotEmpty(),
                SearchMode = input.SearchMode switch
                {
                    DataverseSearchMode.Any => DataverseSearchModeJson.Any,
                    DataverseSearchMode.All => DataverseSearchModeJson.All,
                    _ => null
                },
                SearchType = input.SearchType switch
                {
                    DataverseSearchType.Simple => DataverseSearchTypeJson.Simple,
                    DataverseSearchType.Full => DataverseSearchTypeJson.Full,
                    _ => null
                }
            };

            var request = new DataverseJsonRequest(
                verb: DataverseHttpVerb.Post,
                url: SearchRequestUrl,
                headers: GetAllHeaders(input.CallerObjectId),
                content: searchIn.SerializeOrThrow());

            var result = await httpApi.SendJsonAsync(request, cancellationToken).ConfigureAwait(false);
            return result.MapSuccess(MapSuccess);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to search Dataverse entities");
        }

        static DataverseSearchOut MapSuccess(DataverseJsonResponse response)
        {
            var json = response.Content.DeserializeOrThrow<DataverseSearchJsonOut>();

            return new(
                json.TotalRecordCount,
                json.Value.Map(MapJsonItem));
        }

        static DataverseSearchItem MapJsonItem(DataverseSearchJsonItem jsonItem)
            =>
            new(
                searchScore: jsonItem.SearchScore,
                entityName: jsonItem.EntityName,
                objectId: jsonItem.ObjectId,
                extensionData: jsonItem.ExtensionData?.Select(MapJsonFieldValue).ToArray());

        static KeyValuePair<string, DataverseSearchJsonValue> MapJsonFieldValue(KeyValuePair<string, JsonElement> source)
            =>
            new(
                key: source.Key,
                value: new(source.Value));
    }
}
