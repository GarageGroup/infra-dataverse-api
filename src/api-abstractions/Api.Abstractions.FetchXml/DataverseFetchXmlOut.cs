using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

public sealed record class DataverseFetchXmlOut<TEntityJson>
{
    public DataverseFetchXmlOut(
        FlatArray<TEntityJson> value,
        [AllowNull] string pagingCookie = null)
    {
        Value = value;
        PagingCookie = string.IsNullOrEmpty(pagingCookie) ? null : pagingCookie;
    }

    public FlatArray<TEntityJson> Value { get; }

    public string? PagingCookie { get; }

    public bool MoreRecords { get; init; }
}
