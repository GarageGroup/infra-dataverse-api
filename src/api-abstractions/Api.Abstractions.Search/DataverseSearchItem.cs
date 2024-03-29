using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

public sealed record class DataverseSearchItem
{
    public DataverseSearchItem(
        double searchScore,
        Guid objectId,
        [AllowNull] string entityName,
        FlatArray<KeyValuePair<string, DataverseSearchJsonValue>> extensionData)
    {
        SearchScore = searchScore;
        EntityName = entityName ?? string.Empty;
        ObjectId = objectId;
        ExtensionData = extensionData;
    }

    public DataverseSearchItem(
        double searchScore,
        Guid objectId,
        [AllowNull] string entityName)
    {
        SearchScore = searchScore;
        EntityName = entityName ?? string.Empty;
        ObjectId = objectId;
    }

    public double SearchScore { get; }

    public Guid ObjectId { get; }

    public string EntityName { get; }

    public FlatArray<KeyValuePair<string, DataverseSearchJsonValue>> ExtensionData { get; init; }
}