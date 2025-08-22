using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

public sealed record class DataverseEntitySetGetIn : IDataverseImpersonateIn
{
    public DataverseEntitySetGetIn(string nextLink)
    {
        NextLink = nextLink ?? string.Empty;
        EntityPluralName = string.Empty;
        Filter = string.Empty;
    }

    public DataverseEntitySetGetIn(
        string entityPluralName,
        FlatArray<string> selectFields,
        [AllowNull] string filter = null,
        FlatArray<DataverseExpandedField> expandFields = default,
        FlatArray<DataverseOrderParameter> orderBy = default,
        int? top = null)
    {
        EntityPluralName = entityPluralName ?? string.Empty;
        SelectFields = selectFields;
        ExpandFields = expandFields;
        Filter = filter ?? string.Empty;
        OrderBy = orderBy;
        Top = top;
    }

    public string? NextLink { get; }

    public string EntityPluralName { get; }

    public FlatArray<string> SelectFields { get; }

    public FlatArray<DataverseExpandedField> ExpandFields { get; }

    public string Filter { get; }

    public FlatArray<DataverseOrderParameter> OrderBy { get; }

    public int? Top { get; }

    public int? MaxPageSize { get; init; }

    public string? IncludeAnnotations { get; init; }

    public Guid? CallerObjectId { get; init; }
}