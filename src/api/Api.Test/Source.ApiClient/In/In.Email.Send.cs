using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.Json;
using AutoFixture;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

using DataverseEmailCreateData = TheoryData<DataverseEmailSendIn, DataverseJsonRequest?, DataverseJsonRequest, DataverseEmailCreateJsonOut?>;

partial class ApiClientTestDataSource
{
    public static DataverseEmailCreateData EmailSendInputTestData
    {
        get
        {
            var fixture = new Fixture();
            var data = new DataverseEmailCreateData();

            for (int i = 0; i < 5; i++)
            {
                var emailId = fixture.Create<Guid>();

                data.Add(
                    new(emailId),
                    null,
                    new(
                        verb: DataverseHttpVerb.Post,
                        url: $"/api/data/v9.2/emails({emailId:D})/Microsoft.Dynamics.CRM.SendEmail",
                        headers: default,
                        content: new DataverseEmailSendJsonIn
                        {
                            IssueSend = true
                        }.InnerToJsonContentIn()),
                    null);
            }

            for (int i = 0; i < 20; i++)
            {
                var emailMessage = fixture.Create<MailMessage>();
                var emails = fixture.CreateMany<MailAddress>(3).Select(static m => m.Address).ToArray();
                var memberIds = fixture.CreateMany<Guid>(3).ToArray();
                var emailGuid = fixture.Create<Guid>();

                data.Add(
                    new(
                        subject: emailMessage.Subject,
                        body: emailMessage.Body,
                        sender: new(emails[0]),
                        recipients:
                        [
                            new(emails[1], DataverseEmailRecipientType.ToRecipient),
                            new(
                                emailMember: new(memberIds[0], DataverseEmailMemberType.Account),
                                emailRecipientType: DataverseEmailRecipientType.ToRecipient),
                            new(
                                emailMember: new(memberIds[1], DataverseEmailMemberType.Contact),
                                emailRecipientType: DataverseEmailRecipientType.CcRecipient),
                            new(
                                emailMember: new(memberIds[2], DataverseEmailMemberType.SystemUser),
                                emailRecipientType: DataverseEmailRecipientType.BccRecipient),
                            new(emails[2], DataverseEmailRecipientType.ToRecipient)
                        ])
                    {
                        CallerObjectId = new("bdb05d3f-29c1-48ec-9b0b-5c55e6df669b")
                    },
                    new(
                        verb: DataverseHttpVerb.Post,
                        url: "/api/data/v9.2/emails?$select=activityid",
                        headers:
                        [
                            CreateCallerObjectIdHeader("bdb05d3f-29c1-48ec-9b0b-5c55e6df669b"),
                            new("Accept", "application/json"),
                            new("Prefer", "return=representation")
                        ],
                        content: new DataverseEmailCreateJsonIn
                        {
                            Description = emailMessage.Body,
                            Subject = emailMessage.Subject,
                            ActivityParties =
                            [
                                new()
                                {
                                    ParticipationTypeMask = 1,
                                    AddressUsed = emails[0]
                                },
                                new()
                                {
                                    ParticipationTypeMask = 2,
                                    AddressUsed = emails[1]
                                },
                                new()
                                {
                                    AccountIdParty = $"/accounts({memberIds[0]:D})",
                                    ParticipationTypeMask = 2,
                                },
                                new()
                                {
                                    ContactIdParty = $"/contacts({memberIds[1]:D})",
                                    ParticipationTypeMask = 3,
                                },
                                new()
                                {
                                    SystemUserIdParty = $"/systemusers({memberIds[2]:D})",
                                    ParticipationTypeMask = 4,
                                },
                                new()
                                {
                                    ParticipationTypeMask = 2,
                                    AddressUsed = emails[2]
                                }
                            ],
                            ExtensionData = []
                        }.InnerToJsonContentIn()),
                    new(
                        verb: DataverseHttpVerb.Post,
                        url: $"/api/data/v9.2/emails({emailGuid:D})/Microsoft.Dynamics.CRM.SendEmail",
                        headers:
                        [
                            CreateCallerObjectIdHeader("bdb05d3f-29c1-48ec-9b0b-5c55e6df669b")
                        ],
                        content: new DataverseEmailSendJsonIn
                        {
                            IssueSend = true
                        }.InnerToJsonContentIn()),
                    new()
                    {
                        ActivityId = emailGuid
                    });
            }

            fixture.Register(() => JsonSerializer.SerializeToElement(fixture.Create<string>()));

            for (int i = 0; i < 20; i++)
            {
                var senderEmail = fixture.Create<MailAddress>().Address;
                var recipientEmails = fixture.CreateMany<MailAddress>(i + 1).Select(ma => ma.Address).ToArray();
                var recipients = recipientEmails
                    .Select(static email => new DataverseEmailRecipient(email, DataverseEmailRecipientType.ToRecipient))
                    .ToFlatArray();

                List<DataverseEmailActivityPartyJson> activityParties =
                [
                    new()
                    {
                        AddressUsed = senderEmail,
                        ParticipationTypeMask = 1
                    }
                ];

                activityParties.AddRange(recipientEmails.Select(e => new DataverseEmailActivityPartyJson
                {
                    AddressUsed = e,
                    ParticipationTypeMask = 2
                }));

                var emailMessage2 = fixture.Create<MailMessage>();
                var createdEmailId = fixture.Create<Guid>();

                var extensionData = fixture
                    .CreateMany<KeyValuePair<string, JsonElement>>(i + 1)
                    .DistinctBy(static k => k.Key)
                    .ToDictionary(static k => k.Key, static k => k.Value);

                data.Add(
                    new(
                        subject: emailMessage2.Subject,
                        body: emailMessage2.Body,
                        sender: new(senderEmail),
                        recipients: recipients,
                        extensionData: extensionData.ToFlatArray()),
                    new(
                        verb: DataverseHttpVerb.Post,
                        url: "/api/data/v9.2/emails?$select=activityid",
                        headers: DefaultSendEmailHeaders,
                        content: new DataverseEmailCreateJsonIn
                        {
                            Description = emailMessage2.Body,
                            Subject = emailMessage2.Subject,
                            ActivityParties = activityParties.ToFlatArray(),
                            ExtensionData = extensionData
                        }.InnerToJsonContentIn()),
                    new(
                        verb: DataverseHttpVerb.Post,
                        url: $"/api/data/v9.2/emails({createdEmailId:D})/Microsoft.Dynamics.CRM.SendEmail",
                        headers: default,
                        content: new DataverseEmailSendJsonIn
                        {
                            IssueSend = true
                        }.InnerToJsonContentIn()),
                    new()
                    {
                        ActivityId = createdEmailId
                    });
            }

            return data;
        }
    }
}