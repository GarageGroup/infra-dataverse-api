using System;

namespace GarageGroup.Infra;

public sealed record class DataverseEntityUpdateIn<TInJson> : IDataverseEntityUpdateIn<TInJson>
    where TInJson : notnull
{
    public DataverseEntityUpdateIn(
        string entityPluralName,
        IDataverseEntityKey entityKey,
        FlatArray<string> selectFields,
        TInJson entityData)
    {
        EntityPluralName = entityPluralName ?? string.Empty;
        EntityKey = entityKey;
        SelectFields = selectFields;
        EntityData = entityData;
    }

    public DataverseEntityUpdateIn(
        string entityPluralName,
        IDataverseEntityKey entityKey,
        TInJson entityData)
    {
        EntityPluralName = entityPluralName ?? string.Empty;
        EntityKey = entityKey;
        SelectFields = default;
        EntityData = entityData;
    }

    public string EntityPluralName { get; }

    public FlatArray<string> SelectFields { get; init; }

    public TInJson EntityData { get; }

    public IDataverseEntityKey EntityKey { get; }

    public FlatArray<DataverseExpandedField> ExpandFields { get; init; }

    public bool? SuppressDuplicateDetection { get; init; }

    public DataverseUpdateOperationType OperationType { get; init; }

    public Guid? CallerObjectId { get; init; }
}