using System;
using System.Threading;
using Moq;

namespace GGroupp.Infra.Dataverse.Api.Test;

public sealed partial class DataverseApiClientTest
{
    private static readonly DataverseEntityGetIn SomeDataverseEntityGetInput
        =
        new(
            entityPluralName: "SomeEntities",
            entityKey: new StubEntityKey("Some key"),
            selectFields: new[] { "Some field name" });

    private static readonly DataverseEntitySetGetIn SomeDataverseEntitySetGetInput
        =
        new(
            entityPluralName: "SomeEntities",
            selectFields: new[] { "Some field name" },
            filter: "Some filter",
            orderBy: new DataverseOrderParameter[] { new("one", DataverseOrderDirection.Default) },
            top: 5);

    private static readonly DataverseEntityUpdateIn<StubRequestJson> SomeDataverseEntityUpdateInput
        =
        new(
            entityPluralName: "SomeEntities",
            entityKey: new StubEntityKey("Some key"),
            selectFields: new[] { "Some field name" },
            entityData: new()
            {
                Id = 1,
                Name = "Some name"
            });

    private static readonly DataverseEntityCreateIn<StubRequestJson> SomeDataverseEntityCreateInput
        =
        new(
            entityPluralName: "SomeEntities",
            selectFields: new[] { "Some field name" },
            entityData: new()
            {
                Id = 75,
                Name = "Some entity name"
            });

    private static readonly DataverseEntityDeleteIn SomeDataverseEntityDeleteInput
        =
        new(
            entityPluralName: "SomeEntitiesToDelete",
            entityKey: new StubEntityKey("Some entity key to delete"));

    private static readonly DataverseSearchIn SomeDataverseSearchInput
        =
        new("Some search text")
        {
            OrderBy = new[] { "field 1" },
            Top = 10,
            Skip = 5,
            ReturnTotalRecordCount = false,
            Filter = "Some filter",
            SearchMode = DataverseSearchMode.Any,
            SearchType = DataverseSearchType.Simple
        };

    private static readonly StubResponseJson SomeResponseJson
        =
        new()
        {
            Id = 5,
            Name = "Some name"
        };

    private static readonly DataverseEntitySetJsonGetOut<StubResponseJson> SomeResponseJsonSet
        =
        new()
        {
            Value = new(
                new StubResponseJson
                {
                    Id = 1,
                    Name = "First"
                },
                new StubResponseJson(),
                new StubResponseJson
                {
                    Id = 5
                })
        };

    private static readonly DataverseSearchJsonOut SomeSearchJsonOut
        =
        new()
        {
            TotalRecordCount = 100,
            Value = new(
                new()
                {
                    SearchScore = 100.71,
                    EntityName = "First Enity",
                    ObjectId = Guid.Parse("1f1fb6e6-90a7-4d42-b58d-5e9ac450b37d")
                },
                new()
                {
                    SearchScore = -31798.19,
                    EntityName = "Second",
                    ObjectId = Guid.Parse("d922c096-7ec7-4d0a-a4a4-28b0ced5640e")
                })
        };

    private static readonly DataverseWhoAmIOutJson SomeWhoAmIOutJson
        =
        new()
        {
            BusinessUnitId = Guid.Parse("e0a59544-ba99-43ce-9b57-ed5602c9498c"),
            UserId = Guid.Parse("73efd7b1-4fc4-4793-92f1-aed45ec04843"),
            OrganizationId = Guid.Parse("4ab18f9f-84a5-4b38-a217-cfd0e774cd53")
        };

    private static IDataverseApiClient CreateDataverseApiClient(
        IDataverseHttpApi httpApi, Guid? callerId = null)
    {
        var apiClient = new DataverseApiClient(httpApi);
        return callerId is null ? apiClient : apiClient.Impersonate(callerId.Value);
    }

    private static Mock<IDataverseHttpApi> CreateMockHttpApi<TIn, TOut>(
        Result<TOut?, Failure<DataverseFailureCode>> result)
        where TIn : notnull
    {
        var mock = new Mock<IDataverseHttpApi>();

        _ = mock
            .Setup(p => p.InvokeAsync<TIn, TOut>(It.IsAny<DataverseHttpRequest<TIn>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }
}