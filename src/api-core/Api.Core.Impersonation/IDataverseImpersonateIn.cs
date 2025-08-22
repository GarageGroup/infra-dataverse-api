using System;

namespace GarageGroup.Infra;

public interface IDataverseImpersonateIn
{
    Guid? CallerObjectId { get; }
}