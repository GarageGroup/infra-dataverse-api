using System;

namespace GarageGroup.Infra;

public sealed record class DataverseEntityGetIn : IDataverseImpersonateIn
{
    public DataverseEntityGetIn(string entityPluralName, IDataverseEntityKey entityKey)
    {
        EntityPluralName = entityPluralName ?? string.Empty;
        EntityKey = entityKey;
    }

    public DataverseEntityGetIn(
        string entityPluralName,
        IDataverseEntityKey entityKey,
        FlatArray<string> selectFields)
    {
        EntityPluralName = entityPluralName ?? string.Empty;
        EntityKey = entityKey;
        SelectFields = selectFields;
    }

    public DataverseEntityGetIn(
        string entityPluralName,
        IDataverseEntityKey entityKey,
        FlatArray<string> selectFields,
        FlatArray<DataverseExpandedField> expandFields)
    {
        EntityPluralName = entityPluralName ?? string.Empty;
        EntityKey = entityKey;
        SelectFields = selectFields;
        ExpandFields = expandFields;
    }

    public string EntityPluralName { get; }

    public IDataverseEntityKey EntityKey { get; }

    public FlatArray<string> SelectFields { get; init; }

    public FlatArray<DataverseExpandedField> ExpandFields { get; init; }

    public string? IncludeAnnotations { get; init; }

    public Guid? CallerObjectId { get; init; }
}