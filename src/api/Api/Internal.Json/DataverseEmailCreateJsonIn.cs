using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GarageGroup.Infra;

internal sealed record class DataverseEmailCreateJsonIn
{
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("subject")]
    public string? Subject { get; init; }

    [JsonPropertyName("email_activity_parties")]
    public FlatArray<DataverseEmailActivityPartyJson> ActivityParties { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; init; }
}