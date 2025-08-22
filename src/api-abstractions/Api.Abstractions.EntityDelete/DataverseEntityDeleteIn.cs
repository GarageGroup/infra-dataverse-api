using System;

namespace GarageGroup.Infra;

public sealed record class DataverseEntityDeleteIn : IDataverseEntityDeleteIn
{
    public DataverseEntityDeleteIn(string entityPluralName, IDataverseEntityKey entityKey)
    {
        EntityPluralName = entityPluralName ?? string.Empty;
        EntityKey = entityKey;
    }

    public string EntityPluralName { get; }

    public IDataverseEntityKey EntityKey { get; }

    public Guid? CallerObjectId { get; init; }
}