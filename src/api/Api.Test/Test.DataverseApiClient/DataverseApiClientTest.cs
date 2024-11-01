using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Moq;

namespace GarageGroup.Infra.Dataverse.Api.Test;

public static partial class DataverseApiClientTest
{
    private static readonly JsonSerializerOptions SerializerOptions
        =
        new(JsonSerializerDefaults.Web);

    private static readonly DataverseEntityGetIn SomeDataverseEntityGetInput
        =
        new(
            entityPluralName: "SomeEntities",
            entityKey: new StubEntityKey("Some key"),
            selectFields: new("Some field name"));

    private static readonly DataverseEntitySetGetIn SomeDataverseEntitySetGetInput
        =
        new(
            entityPluralName: "SomeEntities",
            selectFields: new("Some field name"),
            filter: "Some filter",
            orderBy:
            [
                new("one", DataverseOrderDirection.Default)
            ],
            top: 5);

    private static readonly DataverseEntityUpdateIn<StubRequestJson> SomeDataverseEntityUpdateInput
        =
        new(
            entityPluralName: "SomeEntities",
            entityKey: new StubEntityKey("Some key"),
            selectFields: new("Some field name"),
            entityData: new()
            {
                Id = 1,
                Name = "Some name"
            });

    private static readonly DataverseEntityCreateIn<StubRequestJson> SomeDataverseEntityCreateInput
        =
        new(
            entityPluralName: "SomeEntities",
            selectFields: new("Some field name"),
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
            OrderBy = new("field 1"),
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
            Value =
            [
                new()
                {
                    Id = 1,
                    Name = "First"
                },
                new(),
                new()
                {
                    Id = 5
                }
            ]
        };

    private static readonly DataverseSearchJsonOut SomeSearchJsonOut
        =
        new()
        {
            TotalRecordCount = 100,
            Value =
            [
                new()
                {
                    SearchScore = 100.71,
                    EntityName = "First Enity",
                    ObjectId = new("1f1fb6e6-90a7-4d42-b58d-5e9ac450b37d")
                },
                new()
                {
                    SearchScore = -31798.19,
                    EntityName = "Second",
                    ObjectId = new("d922c096-7ec7-4d0a-a4a4-28b0ced5640e")
                }
            ]
        };

    private static readonly DataverseWhoAmIOutJson SomeWhoAmIOutJson
        =
        new()
        {
            BusinessUnitId = new("e0a59544-ba99-43ce-9b57-ed5602c9498c"),
            UserId = new("73efd7b1-4fc4-4793-92f1-aed45ec04843"),
            OrganizationId = new("4ab18f9f-84a5-4b38-a217-cfd0e774cd53")
        };

    private static readonly DataverseEmailCreateJsonOut SomeEmailCreateJsonOut
        =
        new()
        {
            ActivityId = new("9b4a0982-a852-4944-b1db-9b2154d6740b") 
        };

    private static readonly DataverseEmailCreateIn SomeEmailCreateIn
        =
        new(
            subject: "subject",
            body: "body",
            sender: new("email@email.com"),
            recipients:
            [
                new("email2@email.com", DataverseEmailRecipientType.ToRecipient),
                new(
                    emailMember: new(
                        memberId: new("b93c4b03-6067-401d-9a01-b033827d32e9"),
                        memberType: DataverseEmailMemberType.Account),
                    emailRecipientType: DataverseEmailRecipientType.ToRecipient)
            ],
            extensionData: default);

    private static readonly DataverseEmailSendIn SomeEmailSendInWithEmailId
        =
        new(
            emailId: new("9b4a0982-a852-4944-b1db-9b2154d6740b"));

    private static readonly DataverseEmailSendIn SomeEmailSendInWithoutEmailId
        =
        new(
            subject: "subject",
            body: "body",
            sender: new("email@email.com"),
            recipients:
            [
                new(
                    email: "email2@email.com",
                    emailRecipientType: DataverseEmailRecipientType.ToRecipient),
                new(
                    emailMember: new(
                        memberId: new("00d23c27-b73f-402d-9d0b-b590a513d2aa"),
                        memberType: DataverseEmailMemberType.Account),
                    emailRecipientType: DataverseEmailRecipientType.ToRecipient)
            ]);

    private static readonly DataverseChangeSetExecuteIn<object> SomeChangeSetInput
        =
        new(
            requests:
            [
                SomeDataverseEntityCreateInput,
                SomeDataverseEntityUpdateInput,
                SomeDataverseEntityDeleteInput
            ]);

    private static readonly DataverseChangeSetResponse SomeChangeSetResponse
        =
        new(
            responses:
            [
                SomeResponseJson.InnerToJsonResponse(),
                default
            ]);

    private static readonly DataverseEmailCreateJsonOut SomeEmailCreateJson
        =
        new()
        {
            ActivityId = new("73efd7b1-4fc4-4793-92f1-aed45ec04843")
        };

    private static IDataverseApiClient CreateDataverseApiClient(
        IDataverseHttpApi httpApi, IGuidProvider guidProvider, Guid? callerId = null)
    {
        var apiClient = new DataverseApiClient(httpApi, guidProvider);
        return callerId is null ? apiClient : apiClient.Impersonate(callerId.Value);
    }

    private static DataverseJsonResponse InnerToJsonResponse<T>(this T value)
        =>
        new(
            content: new(JsonSerializer.Serialize(value, SerializerOptions)));

    private static IGuidProvider CreateGuidProvider()
        =>
        CreateGuidProvider(
            new("1415478f-b154-4cc0-9687-90b698d77be4"),
            new("33160122-a578-456d-8c8d-e4746296cf1d"));

    private static IGuidProvider CreateGuidProvider(params Guid[] guids)
    {
        var mock = new Mock<IGuidProvider>();
        var queue = new Queue<Guid>(guids);

        _ = mock.Setup(p => p.NewGuid()).Returns(queue.Dequeue);
        return mock.Object;
    }

    private static Mock<IDataverseHttpApi> CreateMockJsonHttpApi(
        in Result<DataverseJsonResponse, Failure<DataverseFailureCode>> result,
        Action<DataverseJsonRequest>? callback = null)
    {
        var mock = new Mock<IDataverseHttpApi>();

        var m = mock
            .Setup(p => p.SendJsonAsync(It.IsAny<DataverseJsonRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        if (callback is not null)
        {
            _ = m.Callback<DataverseJsonRequest, CancellationToken>(
                (r, _) => callback.Invoke(r));
        }

        return mock;
    }

    private static Mock<IDataverseHttpApi> CreateMockEmailHttpApi(
        in Result<DataverseJsonResponse, Failure<DataverseFailureCode>> creationResult,
        in Result<DataverseJsonResponse, Failure<DataverseFailureCode>> sendingResult,
        Action<DataverseJsonRequest>? creationCallback = null)
    {
        var mock = new Mock<IDataverseHttpApi>();

        var m = mock
            .Setup(
                p => p.SendJsonAsync(It.IsAny<DataverseJsonRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(creationResult);

        if (creationCallback is not null)
        {
            _ = m.Callback<DataverseJsonRequest, CancellationToken>(
                (r, _) => creationCallback.Invoke(r));
        }

        _ = mock
            .Setup(
                p => p.SendJsonAsync(
                    It.Is<DataverseJsonRequest>(
                        r => !string.IsNullOrEmpty(r.Url) && r.Url.EndsWith("CRM.SendEmail") == true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sendingResult);

        return mock;
    }

    private static Mock<IDataverseHttpApi> CreateMockChangeSetHttpApi(
        in Result<DataverseChangeSetResponse, Failure<DataverseFailureCode>> result)
    {
        var mock = new Mock<IDataverseHttpApi>();

        _ = mock
            .Setup(p => p.SendChangeSetAsync(It.IsAny<DataverseChangeSetRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }

    private static Mock<IDataverseHttpApi> CreateMockHttpApi(
        Exception exception)
    {
        var mock = new Mock<IDataverseHttpApi>();

        _ = mock
            .Setup(p => p.SendJsonAsync(It.IsAny<DataverseJsonRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        _ = mock
            .Setup(p => p.SendChangeSetAsync(It.IsAny<DataverseChangeSetRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        return mock;
    }
}