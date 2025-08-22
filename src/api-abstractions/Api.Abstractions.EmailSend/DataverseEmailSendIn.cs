using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GarageGroup.Infra;

public sealed record class DataverseEmailSendIn : IDataverseImpersonateIn
{
    public DataverseEmailSendIn(Guid emailId)
    {
        EmailId = emailId;
        Subject = string.Empty;
        Body = string.Empty;
        Sender = new(string.Empty);
    }

    public DataverseEmailSendIn(
        string subject,
        string body,
        DataverseEmailSender sender,
        FlatArray<DataverseEmailRecipient> recipients,
        FlatArray<KeyValuePair<string, JsonElement>> extensionData = default)
    {
        Subject = subject ?? string.Empty;
        Body = body ?? string.Empty;
        Sender = sender;
        Recipients = recipients;
        EmailId = null;
        ExtensionData = extensionData;
    }

    public Guid? EmailId { get; }

    public string Subject { get; }

    public string Body { get; }

    public DataverseEmailSender Sender { get; }

    public FlatArray<DataverseEmailRecipient> Recipients { get; }

    public FlatArray<KeyValuePair<string, JsonElement>> ExtensionData { get; }

    public Guid? CallerObjectId { get; init; }
}