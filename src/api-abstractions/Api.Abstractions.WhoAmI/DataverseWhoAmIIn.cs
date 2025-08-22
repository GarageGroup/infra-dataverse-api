using System;

namespace GarageGroup.Infra;

public readonly record struct DataverseWhoAmIIn : IDataverseImpersonateIn
{
    public Guid? CallerObjectId { get; init; }
}