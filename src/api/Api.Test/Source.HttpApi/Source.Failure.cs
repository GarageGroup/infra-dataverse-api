using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class HttpApiTestDataSource
{
    public static TheoryData<HttpStatusCode, StringContent?, Failure<DataverseFailureCode>> FailureTestData
    {
        get
        {
            var data = new TheoryData<HttpStatusCode, StringContent?, Failure<DataverseFailureCode>>
            {
                {
                    HttpStatusCode.NotFound,
                    null,
                    new(DataverseFailureCode.Unknown, HttpStatusCode.NotFound.GetDefaultFailureMessage())
                }
            };

            var content = new StubFailureJson
            {
                ErrorCode = "0x80060891",
                Message = "Some message"
            }
            .Serialize();

            data.Add(
                HttpStatusCode.InternalServerError,
                new StringContent(content),
                new(DataverseFailureCode.Unknown, content));

            var exceptionFailure = new StubFailureJson
            {
                ExceptionMessage = "Some exception message"
            };

            data.Add(
                HttpStatusCode.BadRequest,
                exceptionFailure.ToJsonContent(),
                new(DataverseFailureCode.Unknown, exceptionFailure.ExceptionMessage));

            var recordNotFoundByEntityKeyFailure = new StubFailureJson
            {
                Error = new()
                {
                    Code = "0x80060891",
                    Description = "Some record was not found"
                }
            };

            data.Add(
                HttpStatusCode.TooManyRequests,
                recordNotFoundByEntityKeyFailure.ToJsonContent(),
                new(DataverseFailureCode.RecordNotFound, recordNotFoundByEntityKeyFailure.Error.Description));

            var objectDoesNotExistFailure = new StubFailureJson
            {
                Failure = new StubFailureInfoJson
                {
                    Code = "0x80040217",
                    Message = "Some object does not exist"
                }
            };

            data.Add(
                HttpStatusCode.Forbidden,
                objectDoesNotExistFailure.ToJsonContent(),
                new(DataverseFailureCode.RecordNotFound, objectDoesNotExistFailure.Failure.Message));

            var picklistValueOutOfRangeFailure = new StubFailureJson
            {
                ErrorCode = "0x8004431A",
                ExceptionMessage = "Some pick list value is out of range"
            };

            data.Add(
                HttpStatusCode.BadRequest,
                picklistValueOutOfRangeFailure.ToJsonContent(),
                new(DataverseFailureCode.PicklistValueOutOfRange, picklistValueOutOfRangeFailure.ExceptionMessage));

            var privilegeDeniedFailure = new StubFailureJson
            {
                Error = new()
                {
                    Code = "0x80040220",
                    Description = "Some user does not hold the necessary privileges"
                }
            };

            data.Add(
                HttpStatusCode.NotAcceptable,
                privilegeDeniedFailure.ToJsonContent(),
                new(DataverseFailureCode.PrivilegeDenied, privilegeDeniedFailure.Error.Description));

            var unManagedIdsAccessDeniedFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x80048306",
                    Message = "Not enough privilege. Some message"
                }
            };

            data.Add(
                HttpStatusCode.InternalServerError,
                unManagedIdsAccessDeniedFailure.ToJsonContent(),
                new(DataverseFailureCode.PrivilegeDenied, unManagedIdsAccessDeniedFailure.Failure.Message));

            var unManagedIdsUserNotEnabledFailure = new StubFailureJson
            {
                ErrorCode = "0x80040225",
                Message = "Some user is disabled"
            };

            data.Add(
                HttpStatusCode.NotFound,
                unManagedIdsUserNotEnabledFailure.ToJsonContent(),
                new(DataverseFailureCode.UserNotEnabled, unManagedIdsUserNotEnabledFailure.Message));

            var userNotAssignedLicenseFailure = new StubFailureJson
            {
                Error = new()
                {
                    Code = "0x8004d24b",
                    Description = "Some user does not have any License"
                }
            };

            data.Add(
                HttpStatusCode.Conflict,
                userNotAssignedLicenseFailure.ToJsonContent(),
                new(DataverseFailureCode.UserNotEnabled, userNotAssignedLicenseFailure.Error.Description));

            var searchableEntityNotFoundFailure = new StubFailureJson
            {
                Error = new()
                {
                    Code = "SearchableEntityNotFound",
                    Description = "Some entity was not found"
                }
            };

            data.Add(
                HttpStatusCode.NotFound,
                searchableEntityNotFoundFailure.ToJsonContent(),
                new(DataverseFailureCode.SearchableEntityNotFound, searchableEntityNotFoundFailure.Error.Description));

            var throttlingFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x8005F103",
                    Message = "Some throttling message"
                }
            };

            data.Add(
                HttpStatusCode.InternalServerError,
                throttlingFailure.ToJsonContent(),
                new(DataverseFailureCode.Throttling, throttlingFailure.Failure.Message));

            var throttlingBurstRequestLimitExceededErrorFailure = new StubFailureJson
            {
                ErrorCode = "0x80072322",
                Message = "Some throttling error message",
                ExceptionMessage = "Some throttling exception message"
            };

            data.Add(
                HttpStatusCode.BadRequest,
                throttlingBurstRequestLimitExceededErrorFailure.ToJsonContent(),
                new(DataverseFailureCode.Throttling, throttlingBurstRequestLimitExceededErrorFailure.Message));

            var throttlingConcurrencyLimitExceededErrorFailure = new StubFailureJson
            {
                Error = new()
                {
                    Code = "0x80072326",
                    Description = "Some throttling concurrency limit was exceeded"
                }
            };

            data.Add(
                HttpStatusCode.BadGateway,
                throttlingConcurrencyLimitExceededErrorFailure.ToJsonContent(),
                new(DataverseFailureCode.Throttling, throttlingConcurrencyLimitExceededErrorFailure.Error.Description));

            var throttlingTimeExceededErrorFailure = new StubFailureJson
            {
                Error = new()
                {
                    Code = "0x80072321",
                    Description = "Some throttling time was exceeded"
                }
            };

            data.Add(
                HttpStatusCode.PaymentRequired,
                throttlingTimeExceededErrorFailure.ToJsonContent(),
                new(DataverseFailureCode.Throttling, throttlingTimeExceededErrorFailure.Error.Description));

            var throttlingCodeFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x80060308",
                    Message = "Some throttling failure"
                }
            };

            data.Add(
                HttpStatusCode.ExpectationFailed,
                throttlingCodeFailure.ToJsonContent(),
                new(DataverseFailureCode.Throttling, throttlingCodeFailure.Failure.Message));

            var duplicateRecordsFoundFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x80040333",
                    Message = "A record was not created or updated because a duplicate of the current record already exists."
                }
            };

            data.Add(
                HttpStatusCode.InternalServerError,
                duplicateRecordsFoundFailure.ToJsonContent(),
                new(DataverseFailureCode.DuplicateRecord, duplicateRecordsFoundFailure.Failure.Message));

            var duplicateRecordEntityKeyFailure = new StubFailureJson
            {
                Error = new()
                {
                    Code = "0x80060892",
                    Description = "Some duplicate record failure message"
                }
            };

            data.Add(
                HttpStatusCode.BadRequest,
                duplicateRecordEntityKeyFailure.ToJsonContent(),
                new(DataverseFailureCode.DuplicateRecord, duplicateRecordEntityKeyFailure.Error.Description));

            var clientPayloadFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x80048d19",
                    Message = "Error identified in Payload provided by the user for Entity :''"
                }
            };

            data.Add(
                HttpStatusCode.BadRequest,
                clientPayloadFailure.ToJsonContent(),
                new(DataverseFailureCode.InvalidPayload, "Error identified in Payload provided by the user for Entity :''"));

            var invalidArgumentFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x80040203",
                    Message = "Invalid argument."
                }
            };

            data.Add(
                HttpStatusCode.BadRequest,
                invalidArgumentFailure.ToJsonContent(),
                new(DataverseFailureCode.InvalidPayload, "Invalid argument."));

            var recipientEmailNotFoundFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x80040b0a",
                    Message = "Recipient email not found failure"
                }
            };

            data.Add(
                HttpStatusCode.BadRequest,
                recipientEmailNotFoundFailure.ToJsonContent(),
                new(DataverseFailureCode.RecipientEmailNotFound, recipientEmailNotFoundFailure.Failure.Message));

            var invalidFileSizeFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x80044a02",
                    Message = "Attachment file size is too big."
                }
            };

            data.Add(
                HttpStatusCode.BadRequest,
                invalidFileSizeFailure.ToJsonContent(),
                new(DataverseFailureCode.InvalidFileSize, "Attachment file size is too big."));

            var isvAbortedFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x80040265",
                    Message = "ISV code aborted the operation."
                }
            };

            data.Add(
                HttpStatusCode.BadRequest,
                invalidFileSizeFailure.ToJsonContent(),
                new(DataverseFailureCode.InvalidFileSize, "Attachment file size is too big."));

            var cannotUpdateBecauseItIsReadOnlyFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x8004022e",
                    Message = "The object cannot be updated because it is read-only."
                }
            };

            data.Add(
                HttpStatusCode.BadRequest,
                cannotUpdateBecauseItIsReadOnlyFailure.ToJsonContent(),
                new(DataverseFailureCode.CannotUpdateBecauseItIsReadOnly, "The object cannot be updated because it is read-only."));

            var unknownFailure = new StubFailureJson
            {
                Failure = new()
                {
                    Code = "0x80060454",
                    Message = "Some unknown failure"
                }
            };

            data.Add(
                HttpStatusCode.BadRequest,
                unknownFailure.ToJsonContent(),
                new(DataverseFailureCode.Unknown, unknownFailure.Failure.Message));

            return data;
        }
    }
}