using System;
using System.Text.Json;

namespace GarageGroup.Infra.Dataverse.Api.Test;

internal static partial class ApiClientTestDataSource
{
    private static readonly JsonSerializerOptions SerializerOptions
        =
        new(JsonSerializerDefaults.Web);

    private static readonly FlatArray<DataverseHttpHeader> DefaultSendEmailHeaders
        =
        [
            new("Accept", "application/json"),
            new("Prefer", "return=representation")
        ];

    private static readonly DataverseHttpHeader PreferRepresentationHeader
        =
        new("Prefer", "return=representation");

    private static DataverseHttpHeader CreateCallerIdHeader(string callerId)
        =>
        new("MSCRMCallerID", callerId);

    private static DataverseHttpHeader CreateCallerObjectIdHeader(string callerId)
        =>
        new("CallerObjectId", callerId);

    private static DataverseHttpHeader CreateSuppressDuplicateDetectionHeader(string value)
        =>
        new("MSCRM.SuppressDuplicateDetection", value);

    private static DataverseJsonContentIn InnerToJsonContentIn<T>(this T value)
        =>
        new(JsonSerializer.Serialize(value, SerializerOptions));
}