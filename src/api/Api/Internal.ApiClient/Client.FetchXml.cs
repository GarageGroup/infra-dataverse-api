using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace GarageGroup.Infra;

internal sealed partial class DataverseApiClient
{
    public ValueTask<Result<DataverseFetchXmlOut<TEntityJson>, Failure<DataverseFailureCode>>> FetchXmlAsync<TEntityJson>(
        DataverseFetchXmlIn input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        return InnerFetchXmlAsync<TEntityJson>(input, cancellationToken);
    }

    private async ValueTask<Result<DataverseFetchXmlOut<TEntityJson>, Failure<DataverseFailureCode>>> InnerFetchXmlAsync<TEntityJson>(
        DataverseFetchXmlIn input, CancellationToken cancellationToken)
    {
        try
        {
            var request = new DataverseJsonRequest(
                verb: DataverseHttpVerb.Get,
                url: BuildFetchXmlUri(input),
                headers: GetAllHeaders(input.CallerObjectId, BuildPreferHeader(input.IncludeAnnotations)).ToFlatArray(),
                content: default);

            var result = await httpApi.SendJsonAsync(request, cancellationToken).ConfigureAwait(false);
            return result.MapSuccess(MapSuccess);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to fetch Dataverse entities");
        }

        static DataverseFetchXmlOut<TEntityJson> MapSuccess(DataverseJsonResponse response)
        {
            var content = response.Content.DeserializeOrThrow<DataverseFetchXmlOutJson<TEntityJson>>();

            var pagingCookie = content.PagingCookie;
            var moreRecords = false;

            if (string.IsNullOrEmpty(pagingCookie) is false)
            {
                moreRecords = true;

                var xmlPagingCookie = new XmlDocument();
                xmlPagingCookie.LoadXml(pagingCookie);

                var innerCookie = xmlPagingCookie.DocumentElement?.Attributes.GetNamedItem(PagingCookieAttributeName)?.Value;
                var decodedPagingCookie = WebUtility.UrlDecode(WebUtility.UrlDecode(innerCookie));

                var htmlEncodedPagingCookie = WebUtility.HtmlEncode(decodedPagingCookie);
                pagingCookie = WebUtility.UrlEncode(htmlEncodedPagingCookie);
            }

            return new(content.Value, pagingCookie)
            {
                MoreRecords = moreRecords
            };
        }
    }
    
    private static string BuildFetchXmlUri(DataverseFetchXmlIn input)
        =>
        BuildDataRequestUrl($"{HttpUtility.UrlEncode(input.EntityPluralName)}?fetchXml={input.FetchXmlQueryString}");
}