using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<DataverseChangeSetExecuteOut<TOut>, Failure<DataverseFailureCode>>> ExecuteChangeSetAsync<TIn, TOut>(
        DataverseChangeSetExecuteIn<TIn> input, CancellationToken cancellationToken = default)
        where TIn : notnull
    {
        if (input.Requests.IsEmpty)
        {
            return new(Result.Success<DataverseChangeSetExecuteOut<TOut>>(default));
        }

        return InnerExecuteChangeSetAsync<TIn, TOut>(input, cancellationToken);
    }

    private async ValueTask<Result<DataverseChangeSetExecuteOut<TOut>, Failure<DataverseFailureCode>>> InnerExecuteChangeSetAsync<TIn, TOut>(
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
                requests: input.Requests.Map(CreateDataverseJsonRequestWithRepresentation));

            var result = await httpApi.SendChangeSetAsync(request, cancellationToken).ConfigureAwait(false);
            return result.MapSuccess(MapSuccess);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ToDataverseFailure(ex, "An unexpected exception was thrown when trying to execute a Dataverse change set");
        }

        static DataverseChangeSetExecuteOut<TOut> MapSuccess(DataverseChangeSetResponse response)
            =>
            new(
                values: response.Responses.Map(GetValue));

        static TOut? GetValue(DataverseJsonResponse response)
            =>
            response.Content.DeserializeOrThrow<TOut>();
    }

    private DataverseJsonRequest CreateDataverseJsonRequestWithRepresentation<TInJson>(IDataverseTransactableIn<TInJson> input)
        where TInJson : notnull
        =>
        input switch
        {
            IDataverseEntityCreateIn<TInJson> @in   => CreateDataverseJsonRequestWithRepresentation(@in),
            IDataverseEntityUpdateIn<TInJson> @in   => CreateDataverseJsonRequestWithRepresentation(@in),
            IDataverseEntityDeleteIn @in            => CreateDataverseJsonRequest(@in),
            _ => throw new InvalidOperationException($"An unexpected change set type: '{input?.GetType().FullName}'")
        };
}