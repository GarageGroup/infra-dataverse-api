using System;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<Failure<DataverseFailureCode>> FailureOutputTestData
        =>
        new()
        {
            {
                Failure.Create(DataverseFailureCode.Unknown, string.Empty)
            },
            {
                Failure.Create(DataverseFailureCode.Unknown, "Some text")
            },
            {
                Failure.Create(DataverseFailureCode.Unauthorized, "Some unauthorized failure")
            },
            {
                Failure.Create(DataverseFailureCode.RecordNotFound, "Some Failure Message")
            },
            {
                Failure.Create(DataverseFailureCode.PicklistValueOutOfRange, "Error message")
            },
            {
                Failure.Create(DataverseFailureCode.UserNotEnabled, "User was not found")
            },
            {
                Failure.Create(DataverseFailureCode.PrivilegeDenied, "Some failure")
            },
            {
                Failure.Create(DataverseFailureCode.Throttling, "Some Throttling Failure")
            },
            {
                Failure.Create(DataverseFailureCode.SearchableEntityNotFound, "Some failure")
            },
            {
                Failure.Create(DataverseFailureCode.InvalidPayload, "Some invalid data")
            },
            {
                Failure.Create(DataverseFailureCode.RecipientEmailNotFound, "Email was not found")
            },
            {
                Failure.Create(DataverseFailureCode.InvalidFileSize, "Some invalid file size message")
            }
        };
}