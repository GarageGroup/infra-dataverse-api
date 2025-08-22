using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<Guid?, DataverseEntitySetGetIn, DataverseJsonRequest> EntitySetGetInputTestData
        =>
        new()
        {
            {
                null,
                new(
                    entityPluralName: "SomeEntities",
                    selectFields: new("field1", "field2"),
                    expandFields:
                    [
                        new("LookupOne", new("field1.1", "field1.2")),
                        new(
                            fieldName: "LookupTwo",
                            selectFields: default,
                            expandFields:
                            [
                                new("field2.1", default)
                            ])
                    ],
                    filter: "id eq 15",
                    orderBy:
                    [
                        new("field3", DataverseOrderDirection.Descending),
                        new("field4"),
                        new("field5", DataverseOrderDirection.Ascending)
                    ],
                    top: 15),
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntities?$select=field1,field2" +
                        "&$expand=LookupOne($select=field1.1,field1.2),LookupTwo($expand=field2.1)" +
                        "&$filter=id eq 15&$orderby=field3 desc,field4,field5 asc&$top=15",
                    headers: default,
                    content: default)
            },
            {
                null,
                new(
                    entityPluralName: "Some Entities",
                    selectFields: new(string.Empty, "field 1"),
                    filter: default,
                    orderBy: default,
                    top: 1)
                    {
                        IncludeAnnotations = "display.*"
                    },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/Some+Entities?$select=field 1&$top=1",
                    headers:
                    [
                        new("Prefer", "odata.include-annotations=display.*")
                    ],
                    content: default)
            },
            {
                null,
                new(
                    entityPluralName: "Some/Entities",
                    selectFields: default,
                    filter: "date gt 2020-01-01")
                    {
                        IncludeAnnotations = "*"
                    },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/Some%2fEntities?$filter=date gt 2020-01-01",
                    headers:
                    [
                        new("Prefer", "odata.include-annotations=*")
                    ],
                    content: default)
            },
            {
                null,
                new(
                    entityPluralName: "SomeEntities",
                    selectFields: new("FieldOne"),
                    filter: string.Empty)
                    {
                        MaxPageSize = 10
                    },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntities?$select=FieldOne",
                    headers:
                    [
                        new("Prefer", "odata.maxpagesize=10")
                    ],
                    content: default)
            },
            {
                new("d44c6578-1f2e-4edd-8897-77aaf8bd524a"),
                new(
                    entityPluralName: "SomeEntities",
                    selectFields: default,
                    expandFields:
                    [
                        new("Field1", new("Lookup Field"))
                    ],
                    filter: default)
                    {
                        MaxPageSize = -5,
                        IncludeAnnotations = "*"
                    },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntities?$expand=Field1($select=Lookup Field)",
                    headers:
                    [
                        CreateCallerIdHeader("d44c6578-1f2e-4edd-8897-77aaf8bd524a"),
                        new("Prefer", "odata.maxpagesize=-5,odata.include-annotations=*")
                    ],
                    content: default)
            },
            {
                null,
                new("http://garage.ru/api/someLink")
                {
                    MaxPageSize = 15,
                    IncludeAnnotations = "*"
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "http://garage.ru/api/someLink",
                    headers:
                    [
                        new("Prefer", "odata.maxpagesize=15,odata.include-annotations=*")
                    ],
                    content: default)
            },
            {
                null,
                new("https://garage.ru/api/someLink")
                {
                    MaxPageSize = 7
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "https://garage.ru/api/someLink",
                    headers:
                    [
                        new("Prefer", "odata.maxpagesize=7")
                    ],
                    content: default)
            },
            {
                null,
                new("/api/someLink")
                {
                    IncludeAnnotations = "*"
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/someLink",
                    headers:
                    [
                        new("Prefer", "odata.include-annotations=*")
                    ],
                    content: default)
            },
            {
                new("be070c0c-3cf5-44a4-8eb2-b9b4a686024b"),
                new("http://garage.ru/api/someLink"),
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "http://garage.ru/api/someLink",
                    headers:
                    [
                        CreateCallerIdHeader("be070c0c-3cf5-44a4-8eb2-b9b4a686024b")
                    ],
                    content: default)
            },
            {
                new("be070c0c-3cf5-44a4-8eb2-b9b4a686024b"),
                new("http://garage.ru/api/someLink")
                {
                    CallerObjectId = new("e0a56877-2598-49a8-8769-b136ccb1c324")
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "http://garage.ru/api/someLink",
                    headers:
                    [
                        CreateCallerObjectIdHeader("e0a56877-2598-49a8-8769-b136ccb1c324")
                    ],
                    content: default)
            }
        };
}