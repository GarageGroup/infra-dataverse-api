using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<Unit, Failure<DataverseFailureCode>>> ExecuteChangeSetAsync<TIn>(
        DataverseChangeSetExecuteIn<TIn> input, CancellationToken cancellationToken = default)
        where TIn : notnull
    {
        if (input.Requests.IsEmpty)
        {
            return new(Result.Success<Unit>(default));
        }

        return InnerExecuteChangeSetAsync(input, cancellationToken);
    }

    private async ValueTask<Result<Unit, Failure<DataverseFailureCode>>> InnerExecuteChangeSetAsync<TIn>(
        DataverseChangeSetExecuteIn<TIn> input, CancellationToken cancellationToken)
        where TIn : notnull
    {
        try
        {
            var request = new DataverseChangeSetRequest(
                url: BuildDataRequestUrl(BatchRelativeUrl),
                batchId: guidProvider.NewGuid(),
                changeSetId: guidProvider.NewGuid(),
                headers: GetAllHeaders(input.CallerObjectId),
                requests: input.Requests.Map(CreateDataverseJsonRequest));

            var result = await httpApi.SendChangeSetAsync(request, cancellationToken).ConfigureAwait(false);
            return result.MapSuccess(Unit.From);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to execute a Dataverse change set");
        }
    }

    private DataverseJsonRequest CreateDataverseJsonRequest<TInJson>(IDataverseTransactableIn<TInJson> input)
        where TInJson : notnull
        =>
        input switch
        {
            IDataverseEntityCreateIn<TInJson> @in   => CreateDataverseJsonRequest(@in),
            IDataverseEntityUpdateIn<TInJson> @in   => CreateDataverseJsonRequest(@in),
            IDataverseEntityDeleteIn @in            => CreateDataverseJsonRequest(@in),
            _ => throw new InvalidOperationException($"An unexpected change set type: '{input?.GetType().FullName}'")
        };
}