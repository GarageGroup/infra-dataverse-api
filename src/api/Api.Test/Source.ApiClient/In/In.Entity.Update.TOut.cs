using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<Guid?, DataverseEntityUpdateIn<IStubRequestJson>, DataverseJsonRequest> EntityUpdateWithTOutInputTestData
        =>
        new()
        {
            {
                new("9fdea890-f164-47c1-bb51-d3865229fa9b"),
                new(
                    entityPluralName: "SomeEntities",
                    entityKey: new StubEntityKey("SomeKey"),
                    selectFields: new("field1", "field2"),
                    entityData: new StubRequestJson
                    {
                        Id = 101,
                        Name = "First request name"
                    })
                {
                    SuppressDuplicateDetection = true,
                    OperationType = DataverseUpdateOperationType.Upsert
                },
                new(
                    verb: DataverseHttpVerb.Patch,
                    url: "/api/data/v9.2/SomeEntities(SomeKey)?$select=field1,field2",
                    headers:
                    [
                        CreateCallerIdHeader("9fdea890-f164-47c1-bb51-d3865229fa9b"),
                        PreferRepresentationHeader,
                        CreateSuppressDuplicateDetectionHeader("true")
                    ],
                    content: new StubRequestJson
                    {
                        Id = 101,
                        Name = "First request name"
                    }.InnerToJsonContentIn())
            },
            {
                null,
                new(
                    entityPluralName: "SomeEntities",
                    entityKey: new StubEntityKey("SomeKey"),
                    selectFields: new(string.Empty, "field 1"),
                    entityData: new StubRequestJson())
                {
                    SuppressDuplicateDetection = false,
                    OperationType = (DataverseUpdateOperationType)(-1)
                },
                new(
                    verb: DataverseHttpVerb.Patch,
                    url: "/api/data/v9.2/SomeEntities(SomeKey)?$select=field 1",
                    headers:
                    [
                        PreferRepresentationHeader,
                        CreateSuppressDuplicateDetectionHeader("false"),
                        new("If-Match", "*")
                    ],
                    content: new StubRequestJson().InnerToJsonContentIn())
            },
            {
                null,
                new(
                    entityPluralName: "Some/Entities",
                    entityKey: new StubEntityKey("Some Key"),
                    selectFields: default,
                    entityData: new StubRequestJson())
                {
                    ExpandFields =
                    [
                        new("Field1", new("Lookup Field"))
                    ],
                    OperationType = DataverseUpdateOperationType.Update
                },
                new(
                    verb: DataverseHttpVerb.Patch,
                    url: "/api/data/v9.2/Some%2fEntities(Some Key)?$expand=Field1($select=Lookup Field)",
                    headers:
                    [
                        PreferRepresentationHeader,
                        new("If-Match", "*")
                    ],
                    content: new StubRequestJson().InnerToJsonContentIn())
            },
            {
                new("9fdea890-f164-47c1-bb51-d3865229fa9b"),
                new(
                    entityPluralName: "SomeEntities",
                    entityKey: new StubEntityKey("SomeKey"),
                    selectFields: new("field1", "field2"),
                    entityData: new StubRequestJson
                    {
                        Id = 101,
                        Name = "First request name"
                    })
                {
                    SuppressDuplicateDetection = true,
                    OperationType = DataverseUpdateOperationType.Upsert,
                    CallerObjectId = new("d7d76c20-675a-4ee8-96d0-755d6a9a2a6f")
                },
                new(
                    verb: DataverseHttpVerb.Patch,
                    url: "/api/data/v9.2/SomeEntities(SomeKey)?$select=field1,field2",
                    headers:
                    [
                        CreateCallerObjectIdHeader("d7d76c20-675a-4ee8-96d0-755d6a9a2a6f"),
                        PreferRepresentationHeader,
                        CreateSuppressDuplicateDetectionHeader("true")
                    ],
                    content: new StubRequestJson
                    {
                        Id = 101,
                        Name = "First request name"
                    }.InnerToJsonContentIn())
            }
        };
}