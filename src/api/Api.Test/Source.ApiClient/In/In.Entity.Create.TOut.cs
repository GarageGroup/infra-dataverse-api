using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<Guid?, DataverseEntityCreateIn<StubRequestJson>, DataverseJsonRequest> EntityCreateWithTOutInputTestData
        =>
        new()
        {
            {
                null,
                new(
                    entityPluralName: "SomeEntities",
                    selectFields: [ "field1", "field2" ],
                    entityData:  new()
                    {
                        Id = 17,
                        Name = "First request name"
                    }),
                new(
                    verb: DataverseHttpVerb.Post,
                    url: "/api/data/v9.2/SomeEntities?$select=field1,field2",
                    headers: new(PreferRepresentationHeader),
                    content: new StubRequestJson
                    {
                        Id = 17,
                        Name = "First request name"
                    }.InnerToJsonContentIn())
            },
            {
                new("cf6678d2-2963-4f14-8dff-21c956ae9695"),
                new(
                    entityPluralName: "SomeEntities",
                    selectFields: [ string.Empty, "field 1" ],
                    entityData: new())
                {
                    ExpandFields =
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
                    SuppressDuplicateDetection = false
                },
                new(
                    verb: DataverseHttpVerb.Post,
                    url: "/api/data/v9.2/SomeEntities?$select=field 1" +
                        "&$expand=LookupOne($select=field1.1,field1.2),LookupTwo($expand=field2.1)",
                    headers:
                    [
                        CreateCallerIdHeader("cf6678d2-2963-4f14-8dff-21c956ae9695"),
                        PreferRepresentationHeader,
                        CreateSuppressDuplicateDetectionHeader("false")
                    ],
                    content: new StubRequestJson().InnerToJsonContentIn())
            },
            {
                null,
                new(
                    entityPluralName: "Some/Entities",
                    selectFields: default,
                    entityData: new())
                {
                    SuppressDuplicateDetection = true
                },
                new(
                    verb: DataverseHttpVerb.Post,
                    url: "/api/data/v9.2/Some%2fEntities",
                    headers:
                    [
                        PreferRepresentationHeader,
                        CreateSuppressDuplicateDetectionHeader("true")
                    ],
                    content: new StubRequestJson().InnerToJsonContentIn())
            },
            {
                new("cf6678d2-2963-4f14-8dff-21c956ae9695"),
                new(
                    entityPluralName: "SomeEntities",
                    selectFields: [ string.Empty, "field 1" ],
                    entityData: new())
                {
                    ExpandFields =
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
                    SuppressDuplicateDetection = false,
                    CallerObjectId = new("021a7a37-29e6-4e1a-afe4-a00fc5a0f476")
                },
                new(
                    verb: DataverseHttpVerb.Post,
                    url: "/api/data/v9.2/SomeEntities?$select=field 1" +
                        "&$expand=LookupOne($select=field1.1,field1.2),LookupTwo($expand=field2.1)",
                    headers:
                    [
                        CreateCallerObjectIdHeader("021a7a37-29e6-4e1a-afe4-a00fc5a0f476"),
                        PreferRepresentationHeader,
                        CreateSuppressDuplicateDetectionHeader("false")
                    ],
                    content: new StubRequestJson().InnerToJsonContentIn())
            },
            {
                null,
                new(
                    entityPluralName: "Some/Entities",
                    selectFields: default,
                    entityData: new())
                {
                    SuppressDuplicateDetection = true,
                    CallerObjectId = new("fb140638-99a7-4d79-95a9-04964e144c83")
                },
                new(
                    verb: DataverseHttpVerb.Post,
                    url: "/api/data/v9.2/Some%2fEntities",
                    headers:
                    [
                        CreateCallerObjectIdHeader("fb140638-99a7-4d79-95a9-04964e144c83"),
                        PreferRepresentationHeader,
                        CreateSuppressDuplicateDetectionHeader("true")
                    ],
                    content: new StubRequestJson().InnerToJsonContentIn())
            }
        };
}