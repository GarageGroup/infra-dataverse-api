using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

public sealed record class DataverseEmailCreateIn : IDataverseImpersonateIn
{
    [SetsRequiredMembers]
    public DataverseEmailCreateIn(
        string subject,
        string body,
        DataverseEmailSender sender,
        FlatArray<DataverseEmailRecipient> recipients,
        FlatArray<KeyValuePair<string, JsonElement>> extensionData)
    {
        Subject = subject ?? string.Empty;
        Body = body ?? string.Empty;
        Sender = sender;
        Recipients = recipients;
        ExtensionData = extensionData;
    }

    [SetsRequiredMembers]
    public DataverseEmailCreateIn(
        string subject,
        string body,
        DataverseEmailSender sender,
        FlatArray<DataverseEmailRecipient> recipients)
    {
        Subject = subject ?? string.Empty;
        Body = body ?? string.Empty;
        Sender = sender;
        Recipients = recipients;
    }

    public DataverseEmailCreateIn(
        string subject,
        string body,
        DataverseEmailSender sender)
    {
        Subject = subject ?? string.Empty;
        Body = body ?? string.Empty;
        Sender = sender;
    }

    public string Subject { get; }

    public string Body { get; }

    public DataverseEmailSender Sender { get; }

    public required FlatArray<DataverseEmailRecipient> Recipients { get; init; }

    public FlatArray<KeyValuePair<string, JsonElement>> ExtensionData { get; init; }

    public Guid? CallerObjectId { get; init; }
}