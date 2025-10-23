using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;

namespace GarageGroup.Infra;

internal sealed partial class DataverseHttpApi : IDataverseHttpApi
{
    private const string HttpVersion = "1.1";

    private readonly HttpMessageHandler messageHandler;

    private readonly Uri dataverseBaseUri;

    private readonly TimeSpan? httpClientTimeOut;

    internal DataverseHttpApi(HttpMessageHandler messageHandler, Uri dataverseBaseUri, TimeSpan? httpClientTimeOut = null)
    {
        this.messageHandler = messageHandler;
        this.dataverseBaseUri = dataverseBaseUri;
        this.httpClientTimeOut = httpClientTimeOut;
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient(messageHandler, disposeHandler: false)
        {
            BaseAddress = dataverseBaseUri,
        };

        if (httpClientTimeOut is not null)
        {
            httpClient.Timeout = httpClientTimeOut.Value;
        }

        return httpClient;
    }

    private static DataverseFailureJson? DeserializeFailure(string? body, string? mediaType)
    {
        if (mediaType is MediaTypeNames.Application.Json && string.IsNullOrEmpty(body) is false)
        {
            return new DataverseJsonContentOut(body).DeserializeOrThrow<DataverseFailureJson>();
        }

        return null;
    }

    private static Failure<DataverseFailureCode> ToDataverseFailure(
        DataverseFailureJson? failureJson, string? body, HttpStatusCode statusCode)
    {
        if (failureJson is null)
        {
            return new(GetFailureCode(null), GetFailureMessage(null));
        }

        var value = failureJson.Value;

        if (value.Failure is not null)
        {
            return new(GetFailureCode(value.Failure.Code), GetFailureMessage(value.Failure.Message));
        }

        if (value.Error is not null)
        {
            return new(GetFailureCode(value.Error.Code), GetFailureMessage(value.Error.Description));
        }

        var failureCode = GetFailureCode(value.ErrorCode);

        if (string.IsNullOrEmpty(value.Message) is false)
        {
            return new(failureCode, value.Message);
        }

        return new(failureCode, GetFailureMessage(value.ExceptionMessage));

        string GetFailureMessage(string? message)
        {
            if (string.IsNullOrEmpty(message) is false)
            {
                return message;
            }

            if (string.IsNullOrEmpty(body) is false)
            {
                return body;
            }

            return GetStatusCodeFailureMessage(statusCode);
        }

        DataverseFailureCode GetFailureCode(string? code)
            =>
            ToDataverseFailureCode(statusCode, code);
    }

    private static string GetStatusCodeFailureMessage(HttpStatusCode statusCode)
        =>
        $"An unexpected Dataverse respose status: {statusCode}";

    private static DataverseFailureCode ToDataverseFailureCode(HttpStatusCode statusCode, string? code)
    {
        if (statusCode is HttpStatusCode.Unauthorized)
        {
            return DataverseFailureCode.Unauthorized;
        }

        return code switch
        {
            "0x80060891" or "0x80040217" => DataverseFailureCode.RecordNotFound,
            "0x8004431A" => DataverseFailureCode.PicklistValueOutOfRange,
            "0x80040220" or "0x80048306" => DataverseFailureCode.PrivilegeDenied,
            "0x80040225" or "0x8004d24b" => DataverseFailureCode.UserNotEnabled,
            "SearchableEntityNotFound" => DataverseFailureCode.SearchableEntityNotFound,
            "0x8005F103" or "0x80072322" or "0x80072326" or "0x80072321" or "0x80060308" => DataverseFailureCode.Throttling,
            "0x80040333" or "0x80060892" => DataverseFailureCode.DuplicateRecord,
            "0x80040203" or "0x80048d19" => DataverseFailureCode.InvalidPayload,
            "0x80040b0a" => DataverseFailureCode.RecipientEmailNotFound,
            "0x80044a02" => DataverseFailureCode.InvalidFileSize,
            "0x80040265" => DataverseFailureCode.IsvAborted,
            "0x8004022e" => DataverseFailureCode.CannotUpdateBecauseItIsReadOnly,
            _ => DataverseFailureCode.Unknown
        };
    }
}