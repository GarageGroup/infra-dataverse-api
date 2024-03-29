using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GarageGroup.Infra.Dataverse.Api.Test;

internal sealed record class StubHttpRequestData
{
    public StubHttpRequestData(HttpMethod method, string? requestUrl)
    {
        Method = method;
        RequestUrl = requestUrl;
    }
    
    public HttpMethod Method { get; init; }

    public string? RequestUrl { get; init; }

    public FlatArray<KeyValuePair<string, string>> Headers { get; init; }

    public string? Content { get; init; }
}