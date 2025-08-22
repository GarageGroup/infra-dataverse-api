using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<Guid?, DataverseFetchXmlIn, DataverseJsonRequest> FetchXmlInputTestData
        =>
        new()
        {
            {
                null,
                new("Some Entity", "SomeXml")
                {
                    IncludeAnnotations = "Some Annotations",
                    CallerObjectId = null
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/Some+Entity?fetchXml=SomeXml",
                    headers:
                    [
                        new("Prefer", "odata.include-annotations=Some Annotations")
                    ],
                    content: default)
            },
            {
                null,
                new("SomeEntity", "SomeXml")
                {
                    IncludeAnnotations = null,
                    CallerObjectId = null
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntity?fetchXml=SomeXml",
                    headers: default,
                    content: default)
            },
            {
                null,
                new("SomeEntity", "SomeXml")
                {
                    IncludeAnnotations = null,
                    CallerObjectId = new("3a33330e-6b50-43f1-b309-3f76990e586b")
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntity?fetchXml=SomeXml",
                    headers:
                    [
                        CreateCallerObjectIdHeader("3a33330e-6b50-43f1-b309-3f76990e586b")
                    ],
                    content: default)
            },
            {
                new("5e2e826e-29c9-4519-8aed-495aa14b8d83"),
                new("SomeEntity", "SomeXml")
                {
                    IncludeAnnotations = "Some Annotations",
                    CallerObjectId = null
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntity?fetchXml=SomeXml",
                    headers:
                    [
                        CreateCallerIdHeader("5e2e826e-29c9-4519-8aed-495aa14b8d83"),
                        new("Prefer", "odata.include-annotations=Some Annotations")
                    ],
                    content: default)
            },
            {
                new("5e2e826e-29c9-4519-8aed-495aa14b8d83"),
                new("SomeEntity", "SomeXml")
                {
                    IncludeAnnotations = null,
                    CallerObjectId = null
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntity?fetchXml=SomeXml",
                    headers:
                    [
                        CreateCallerIdHeader("5e2e826e-29c9-4519-8aed-495aa14b8d83")
                    ],
                    content: default)
            },
            {
                new("5e2e826e-29c9-4519-8aed-495aa14b8d83"),
                new("SomeEntity", "SomeXml")
                {
                    IncludeAnnotations = "Some Annotations",
                    CallerObjectId = new("bc957632-5a7f-48ed-9c44-a800cfefcf8b")
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntity?fetchXml=SomeXml",
                    headers:
                    [
                        CreateCallerObjectIdHeader("bc957632-5a7f-48ed-9c44-a800cfefcf8b"),
                        new("Prefer", "odata.include-annotations=Some Annotations")
                    ],
                    content: default)
            },
            {
                new("5e2e826e-29c9-4519-8aed-495aa14b8d83"),
                new("SomeEntity", "SomeXml")
                {
                    IncludeAnnotations = null,
                    CallerObjectId = new("bc957632-5a7f-48ed-9c44-a800cfefcf8b")
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntity?fetchXml=SomeXml",
                    headers:
                    [
                        CreateCallerObjectIdHeader("bc957632-5a7f-48ed-9c44-a800cfefcf8b")
                    ],
                    content: default)
            }
        };
}