using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public async ValueTask<Result<Unit, Failure<Unit>>> PingAsync(Unit _, CancellationToken cancellationToken)
    {
        try
        {
            var request = new DataverseJsonRequest(
                verb: DataverseHttpVerb.Get,
                url: BuildDataRequestUrl(WhoAmIRelativeUrl),
                headers: GetAllHeaders(),
                content: default);

            var result = await httpApi.SendJsonAsync(request, cancellationToken).ConfigureAwait(false);
            return result.Map(Unit.From, InnerMapFailure);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ex.ToFailure("An unexpected exception was thrown when trying to ping a Dataverse API");
        }

        static Failure<Unit> InnerMapFailure(Failure<DataverseFailureCode> failure)
            =>
            failure.WithFailureCode<Unit>(default);
    }
}