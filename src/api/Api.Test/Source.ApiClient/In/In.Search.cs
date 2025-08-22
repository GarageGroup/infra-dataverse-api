using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<Guid?, DataverseSearchIn, DataverseJsonRequest> SearchInputTestData
        =>
        new()
        {
            {
                null,
                new("Some first text")
                {
                    OrderBy = new("field 1"),
                    Top = 10,
                    Skip = 5,
                    ReturnTotalRecordCount = false,
                    Filter = "Some filter",
                    Facets = new("one", "two", string.Empty),
                    Entities = new("first", "second", string.Empty, "third"),
                    SearchMode = DataverseSearchMode.Any,
                    SearchType = DataverseSearchType.Full
                },
                new(
                    verb: DataverseHttpVerb.Post,
                    url: "/api/search/v1.0/query",
                    headers: default,
                    content: new DataverseSearchJsonIn
                    {
                        Search = "Some first text",
                        OrderBy = new("field 1"),
                        Top = 10,
                        Skip = 5,
                        ReturnTotalRecordCount = false,
                        Filter = "Some filter",
                        Facets = new("one", "two"),
                        Entities = new("first", "second", "third"),
                        SearchMode = DataverseSearchModeJson.Any,
                        SearchType = DataverseSearchTypeJson.Full
                    }.InnerToJsonContentIn())
            },
            {
                new("aa087335-0897-4d6e-82cb-0f07cb6fc2f4"),
                new("Some second text")
                {
                    OrderBy = new("field 1", string.Empty),
                    SearchMode = DataverseSearchMode.All,
                    SearchType = DataverseSearchType.Simple
                },
                new(
                    verb: DataverseHttpVerb.Post,
                    url: "/api/search/v1.0/query",
                    headers:
                    [
                        CreateCallerIdHeader("aa087335-0897-4d6e-82cb-0f07cb6fc2f4")
                    ],
                    content: new DataverseSearchJsonIn
                    {
                        Search = "Some second text",
                        OrderBy = new("field 1"),
                        SearchMode = DataverseSearchModeJson.All,
                        SearchType = DataverseSearchTypeJson.Simple
                    }.InnerToJsonContentIn())
            },
            {
                null,
                new(string.Empty),
                new(
                    verb: DataverseHttpVerb.Post,
                    url: "/api/search/v1.0/query",
                    headers: default,
                    content: new DataverseSearchJsonIn
                    {
                        Search = string.Empty
                    }.InnerToJsonContentIn())
            },
            {
                new("aa087335-0897-4d6e-82cb-0f07cb6fc2f4"),
                new("Some second text")
                {
                    OrderBy = new("field 1", string.Empty),
                    SearchMode = DataverseSearchMode.All,
                    SearchType = DataverseSearchType.Simple,
                    CallerObjectId = new("722fd46e-62ff-4de2-9b82-cd64a95977c4")
                },
                new(
                    verb: DataverseHttpVerb.Post,
                    url: "/api/search/v1.0/query",
                    headers:
                    [
                        CreateCallerObjectIdHeader("722fd46e-62ff-4de2-9b82-cd64a95977c4")
                    ],
                    content: new DataverseSearchJsonIn
                    {
                        Search = "Some second text",
                        OrderBy = new("field 1"),
                        SearchMode = DataverseSearchModeJson.All,
                        SearchType = DataverseSearchTypeJson.Simple
                    }.InnerToJsonContentIn())
            }
        };
}