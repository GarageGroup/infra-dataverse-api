using System;

namespace GarageGroup.Infra;

public sealed record class DataverseEntityCreateIn<TInJson> : IDataverseEntityCreateIn<TInJson>
    where TInJson : notnull
{
    public DataverseEntityCreateIn(string entityPluralName, FlatArray<string> selectFields, TInJson entityData)
    {
        EntityPluralName = entityPluralName ?? string.Empty;
        SelectFields = selectFields;
        EntityData = entityData;
    }

    public DataverseEntityCreateIn(string entityPluralName, TInJson entityData)
    {
        EntityPluralName = entityPluralName ?? string.Empty;
        EntityData = entityData;
    }

    public string EntityPluralName { get; }

    public FlatArray<string> SelectFields { get; init; }

    public TInJson EntityData { get; }

    public FlatArray<DataverseExpandedField> ExpandFields { get; init; }

    public bool? SuppressDuplicateDetection { get; init; }

    public Guid? CallerObjectId { get; init; }
}