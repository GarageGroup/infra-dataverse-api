using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<Guid?, DataverseEntityGetIn, DataverseJsonRequest> EntityGetInputTestData
        =>
        new()
        {
            {
                new("18945ff7-9433-4e74-a403-abd6db25ef27"),
                new(
                    entityPluralName: "SomeEntities",
                    entityKey: new StubEntityKey("SomeKey"),
                    selectFields: new("field1", "field2"),
                    expandFields:
                    [
                        new("LookupOne", new("field1.1", "field1.2", string.Empty)),
                        new(string.Empty, new("field 2.1", "field 2.2")),
                        new("LookupThree", new("field3.1"))
                    ]),
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntities(SomeKey)?$select=field1,field2&" +
                        "$expand=LookupOne($select=field1.1,field1.2),LookupThree($select=field3.1)",
                    headers:
                    [
                        CreateCallerIdHeader("18945ff7-9433-4e74-a403-abd6db25ef27")
                    ],
                    content: default)
            },
            {
                null,
                new(
                    entityPluralName: "SomeEntities",
                    entityKey: new StubEntityKey("SomeKey"),
                    selectFields: new(string.Empty, "field 1"))
                    {
                        IncludeAnnotations = "display.*"
                    },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntities(SomeKey)?$select=field 1",
                    headers:
                    [
                        new("Prefer", "odata.include-annotations=display.*")
                    ],
                    content: default)
            },
            {
                new("18945ff7-9433-4e74-a403-abd6db25ef27"),
                new(
                    entityPluralName: "Some/Entities",
                    entityKey: new StubEntityKey("Some=Key"),
                    selectFields: default,
                    expandFields:
                    [
                        new(
                            fieldName: "SomeField",
                            selectFields: default,
                            expandFields:
                            [
                                new(
                                    fieldName: "Lookup1",
                                    selectFields: new("Field1", "Field2"),
                                    expandFields:
                                    [
                                        new("Lookup3", default)
                                    ])
                            ])
                    ])
                    {
                        IncludeAnnotations = "*"
                    },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/Some%2fEntities(Some=Key)?$expand=SomeField($expand=Lookup1($select=Field1,Field2;$expand=Lookup3))",
                    headers:
                    [
                        CreateCallerIdHeader("18945ff7-9433-4e74-a403-abd6db25ef27"),
                        new DataverseHttpHeader("Prefer", "odata.include-annotations=*")
                    ],
                    content: default)
            },
            {
                new("18945ff7-9433-4e74-a403-abd6db25ef27"),
                new(
                    entityPluralName: "Some/Entities",
                    entityKey: new StubEntityKey("Some=Key"),
                    selectFields: default,
                    expandFields: default)
                    {
                        IncludeAnnotations = "*"
                    },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/Some%2fEntities(Some=Key)",
                    headers:
                    [
                        CreateCallerIdHeader("18945ff7-9433-4e74-a403-abd6db25ef27"),
                        new("Prefer", "odata.include-annotations=*")
                    ],
                    content: default)
            },
            {
                new("18945ff7-9433-4e74-a403-abd6db25ef27"),
                new(
                    entityPluralName: "SomeEntities",
                    entityKey: new StubEntityKey("SomeKey"),
                    selectFields: new("field1", "field2"),
                    expandFields:
                    [
                        new("LookupOne")
                    ]),
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntities(SomeKey)?$select=field1,field2&$expand=LookupOne",
                    headers:
                    [
                        CreateCallerIdHeader("18945ff7-9433-4e74-a403-abd6db25ef27")
                    ],
                    content: default)
            },
            {
                new("18945ff7-9433-4e74-a403-abd6db25ef27"),
                new(
                    entityPluralName: "SomeEntities",
                    entityKey: new StubEntityKey("SomeKey"),
                    selectFields: new("field1", "field2"),
                    expandFields:
                    [
                        new("LookupOne")
                    ])
                {
                    CallerObjectId = new("510cd0e0-9e82-41fb-8ec0-e1f14d9a1842")
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/SomeEntities(SomeKey)?$select=field1,field2&$expand=LookupOne",
                    headers:
                    [
                        CreateCallerObjectIdHeader("510cd0e0-9e82-41fb-8ec0-e1f14d9a1842")
                    ],
                    content: default)
            }
        };
}