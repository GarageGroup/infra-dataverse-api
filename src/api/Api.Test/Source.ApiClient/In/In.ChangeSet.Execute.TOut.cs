using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

using DataverseObjectSetExecuteIn = DataverseChangeSetExecuteIn<object>;

partial class ApiClientTestDataSource
{
    public static TheoryData<Guid?, Guid, Guid, DataverseObjectSetExecuteIn, DataverseChangeSetRequest> ChangeSetExecuteWithTOutInputTestData
        =>
        new()
        {
            {
                null,
                new("ce1a0575-e83e-4b5f-865a-8d501df4e40c"),
                new("d7762c7e-7774-4771-a5dc-64578840a210"),
                new(
                    requests:
                    [
                        new DataverseEntityCreateIn<StubRequestJson>(
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
                        }
                    ]),
                new(
                    url: "/api/data/v9.2/$batch",
                    batchId: new("ce1a0575-e83e-4b5f-865a-8d501df4e40c"),
                    changeSetId: new("d7762c7e-7774-4771-a5dc-64578840a210"),
                    headers: default,
                    requests:
                    [
                        new(
                            verb: DataverseHttpVerb.Post,
                            url: "/api/data/v9.2/SomeEntities?$select=field 1" +
                                "&$expand=LookupOne($select=field1.1,field1.2),LookupTwo($expand=field2.1)",
                            headers: new(
                                PreferRepresentationHeader,
                                CreateSuppressDuplicateDetectionHeader("false")),
                        content: new StubRequestJson().InnerToJsonContentIn())
                    ])
            },
            {
                new("e97607f1-8716-4efc-947c-c631d81be853"),
                new("1f23da9e-be1b-4967-9aff-f0723876d275"),
                new("f2882787-a456-4ff5-bb13-87fdb377849f"),
                new(
                    requests:
                    [
                        new DataverseEntityUpdateIn<StubRequestJson>(
                            entityPluralName: "Some/Entities",
                            entityKey: new StubEntityKey("Some Key"),
                            selectFields: default,
                            entityData: new())
                        {
                            ExpandFields =
                            [
                                new("Field1", new("Lookup Field"))
                            ],
                            OperationType = DataverseUpdateOperationType.Upsert
                        },
                        new DataverseEntityDeleteIn(
                            entityPluralName: "SomeEntities",
                            entityKey: new StubEntityKey("SomeKey"))
                    ]),
                new(
                    url: "/api/data/v9.2/$batch",
                    batchId: new("1f23da9e-be1b-4967-9aff-f0723876d275"),
                    changeSetId: new("f2882787-a456-4ff5-bb13-87fdb377849f"),
                    headers:
                    [
                        CreateCallerIdHeader("e97607f1-8716-4efc-947c-c631d81be853")
                    ],
                    requests:
                    [
                        new(
                            verb: DataverseHttpVerb.Patch,
                            url: "/api/data/v9.2/Some%2fEntities(Some Key)?$expand=Field1($select=Lookup Field)",
                            headers:
                            [
                                CreateCallerIdHeader("e97607f1-8716-4efc-947c-c631d81be853"),
                                PreferRepresentationHeader
                            ],
                            content: new StubRequestJson().InnerToJsonContentIn()),
                        new(
                            verb: DataverseHttpVerb.Delete,
                            url: "/api/data/v9.2/SomeEntities(SomeKey)",
                            headers:
                            [
                                CreateCallerIdHeader("e97607f1-8716-4efc-947c-c631d81be853")
                            ],
                            content: default)
                    ])
            },
            {
                new("e97607f1-8716-4efc-947c-c631d81be853"),
                new("1f23da9e-be1b-4967-9aff-f0723876d275"),
                new("f2882787-a456-4ff5-bb13-87fdb377849f"),
                new(
                    requests:
                    [
                        new DataverseEntityUpdateIn<StubRequestJson>(
                            entityPluralName: "Some/Entities",
                            entityKey: new StubEntityKey("Some Key"),
                            selectFields: default,
                            entityData: new())
                        {
                            ExpandFields =
                            [
                                new("Field1", new("Lookup Field"))
                            ],
                            OperationType = DataverseUpdateOperationType.Upsert
                        },
                        new DataverseEntityDeleteIn(
                            entityPluralName: "SomeEntities",
                            entityKey: new StubEntityKey("SomeKey"))
                    ])
                {
                    CallerObjectId = new("0921e18b-ff6c-4124-878e-2e9434e0b319")
                },
                new(
                    url: "/api/data/v9.2/$batch",
                    batchId: new("1f23da9e-be1b-4967-9aff-f0723876d275"),
                    changeSetId: new("f2882787-a456-4ff5-bb13-87fdb377849f"),
                    headers:
                    [
                        CreateCallerObjectIdHeader("0921e18b-ff6c-4124-878e-2e9434e0b319")
                    ],
                    requests:
                    [
                        new(
                            verb: DataverseHttpVerb.Patch,
                            url: "/api/data/v9.2/Some%2fEntities(Some Key)?$expand=Field1($select=Lookup Field)",
                            headers:
                            [
                                CreateCallerIdHeader("e97607f1-8716-4efc-947c-c631d81be853"),
                                PreferRepresentationHeader
                            ],
                            content: new StubRequestJson().InnerToJsonContentIn()),
                        new(
                            verb: DataverseHttpVerb.Delete,
                            url: "/api/data/v9.2/SomeEntities(SomeKey)",
                            headers:
                            [
                                CreateCallerIdHeader("e97607f1-8716-4efc-947c-c631d81be853")
                            ],
                            content: default)
                    ])
            }
        };
}