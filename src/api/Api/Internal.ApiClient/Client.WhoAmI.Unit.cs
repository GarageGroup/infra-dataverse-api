using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra;

partial class DataverseApiClient
{
    public ValueTask<Result<DataverseWhoAmIOut, Failure<DataverseFailureCode>>> WhoAmIAsync(
        Unit input, CancellationToken cancellationToken = default)
        =>
        InnerWhoAmIAsync(default, cancellationToken);
}