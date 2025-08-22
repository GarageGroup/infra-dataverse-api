using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<Guid?, DataverseWhoAmIIn, DataverseJsonRequest> WhoAmIInputTestData
        =>
        new()
        {
            {
                null,
                default,
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/WhoAmI",
                    headers: default,
                    content: default)
            },
            {
                new("68731777-a43a-454c-bd93-7680a887e6eb"),
                default,
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/WhoAmI",
                    headers:
                    [
                        CreateCallerIdHeader("68731777-a43a-454c-bd93-7680a887e6eb")
                    ],
                    content: default)
            },
            {
                new("68731777-a43a-454c-bd93-7680a887e6eb"),
                new()
                {
                    CallerObjectId = new("696e4cf7-6798-451e-a5c3-c86590854443")
                },
                new(
                    verb: DataverseHttpVerb.Get,
                    url: "/api/data/v9.2/WhoAmI",
                    headers:
                    [
                        CreateCallerObjectIdHeader("696e4cf7-6798-451e-a5c3-c86590854443")
                    ],
                    content: default)
            }
        };
}