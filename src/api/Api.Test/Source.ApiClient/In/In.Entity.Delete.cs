using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<Guid?, DataverseEntityDeleteIn, DataverseJsonRequest> EntityDeleteInputTestData
        =>
        new()
        {
            {
                new("91526fc6-1491-4ee9-8b7a-a4ed536de862"),
                new(
                    entityPluralName: "SomeEntities",
                    entityKey: new StubEntityKey("SomeKey")),
                new(
                    verb: DataverseHttpVerb.Delete,
                    url: "/api/data/v9.2/SomeEntities(SomeKey)",
                    headers:
                    [
                        CreateCallerIdHeader("91526fc6-1491-4ee9-8b7a-a4ed536de862")
                    ],
                    content: default)
            },
            {
                null,
                new(
                    entityPluralName: "Some/Entities",
                    entityKey: new StubEntityKey("Some=Key")),
                new(
                    verb: DataverseHttpVerb.Delete,
                    url: "/api/data/v9.2/Some%2fEntities(Some=Key)",
                    headers: default,
                    content: default)
            },
            {
                new("91526fc6-1491-4ee9-8b7a-a4ed536de862"),
                new(
                    entityPluralName: "SomeEntities",
                    entityKey: new StubEntityKey("SomeKey"))
                {
                    CallerObjectId = new("8621a868-d9da-4ef8-823e-cf20ed360992")
                },
                new(
                    verb: DataverseHttpVerb.Delete,
                    url: "/api/data/v9.2/SomeEntities(SomeKey)",
                    headers:
                    [
                        CreateCallerObjectIdHeader("8621a868-d9da-4ef8-823e-cf20ed360992")
                    ],
                    content: default)
            },
            {
                null,
                new(
                    entityPluralName: "Some/Entities",
                    entityKey: new StubEntityKey("Some=Key"))
                {
                    CallerObjectId = new("0fa35c93-90d9-4963-be0e-2e848016c87c")
                },
                new(
                    verb: DataverseHttpVerb.Delete,
                    url: "/api/data/v9.2/Some%2fEntities(Some=Key)",
                    headers:
                    [
                        CreateCallerObjectIdHeader("0fa35c93-90d9-4963-be0e-2e848016c87c")
                    ],
                    content: default)
            }
        };
}