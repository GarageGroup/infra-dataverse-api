using System.Text.Json.Serialization;

namespace GGroupp.Infra;

internal sealed record DataverseFailureJson
{
    [JsonPropertyName("error")]
    public DataverseFailureInfoJson? Error { get; init; }
}