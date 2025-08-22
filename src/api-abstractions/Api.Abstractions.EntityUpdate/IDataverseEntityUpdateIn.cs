using System;

namespace GarageGroup.Infra;

public interface IDataverseEntityUpdateIn<out TInJson> : IDataverseTransactableIn<TInJson>, IDataverseImpersonateIn
    where TInJson : notnull
{
    string EntityPluralName { get; }

    FlatArray<string> SelectFields { get; }

    TInJson EntityData { get; }

    IDataverseEntityKey EntityKey { get; }

    FlatArray<DataverseExpandedField> ExpandFields { get; }

    bool? SuppressDuplicateDetection { get; }

    DataverseUpdateOperationType OperationType { get; }

    TInJson? IDataverseTransactableIn<TInJson>.Entity
        =>
        EntityData;
}