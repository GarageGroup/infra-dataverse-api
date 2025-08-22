using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra;

public interface IDataverseWhoAmISupplier
{
    ValueTask<Result<DataverseWhoAmIOut, Failure<DataverseFailureCode>>> WhoAmIAsync(
        CancellationToken cancellationToken = default)
        =>
        WhoAmIAsync(default(DataverseWhoAmIIn), cancellationToken);

    ValueTask<Result<DataverseWhoAmIOut, Failure<DataverseFailureCode>>> WhoAmIAsync(
        Unit input, CancellationToken cancellationToken = default);

    ValueTask<Result<DataverseWhoAmIOut, Failure<DataverseFailureCode>>> WhoAmIAsync(
        DataverseWhoAmIIn input, CancellationToken cancellationToken = default);
}