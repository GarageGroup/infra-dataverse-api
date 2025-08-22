using System;

namespace GarageGroup.Infra;

public sealed record class DataverseFetchXmlIn : IDataverseImpersonateIn
{
    public DataverseFetchXmlIn(string entityPluralName, string fetchXmlQueryString)
    {
        FetchXmlQueryString = fetchXmlQueryString ?? string.Empty;
        EntityPluralName = entityPluralName ?? string.Empty;
    }

    public string FetchXmlQueryString { get; }

    public string EntityPluralName { get; }

    public string? IncludeAnnotations { get; init; }

    public Guid? CallerObjectId { get; init; }
}