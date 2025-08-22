using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<Unit, Failure<DataverseFailureCode>>> DeleteEntityAsync(
        DataverseEntityDeleteIn input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        return InnerDeleteEntityAsync(input, cancellationToken);
    }

    private async ValueTask<Result<Unit, Failure<DataverseFailureCode>>> InnerDeleteEntityAsync(
        DataverseEntityDeleteIn input, CancellationToken cancellationToken)
    {
        try
        {
            var request = CreateDataverseJsonRequest(input);
            var result = await httpApi.SendJsonAsync(request, cancellationToken).ConfigureAwait(false);

            return result.MapSuccess(Unit.From);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to delete a Dataverse entity");
        }
    }

    private DataverseJsonRequest CreateDataverseJsonRequest(IDataverseEntityDeleteIn input)
    {
        var encodedPluralName = HttpUtility.UrlEncode(input.EntityPluralName);

        return new(
            verb: DataverseHttpVerb.Delete,
            url: BuildDataRequestUrl($"{encodedPluralName}({input.EntityKey.Value})"),
            headers: GetAllHeaders(input.CallerObjectId),
            content: default);
    }
}