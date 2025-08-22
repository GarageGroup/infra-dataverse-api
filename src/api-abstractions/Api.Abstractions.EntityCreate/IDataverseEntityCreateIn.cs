using System;

namespace GarageGroup.Infra;

public interface IDataverseEntityCreateIn<out TInJson> : IDataverseTransactableIn<TInJson>, IDataverseImpersonateIn
    where TInJson : notnull
{
    string EntityPluralName { get; }

    FlatArray<string> SelectFields { get; }

    TInJson EntityData { get; }

    FlatArray<DataverseExpandedField> ExpandFields { get; }

    bool? SuppressDuplicateDetection { get; }

    TInJson? IDataverseTransactableIn<TInJson>.Entity
        =>
        EntityData;
}
