using System;
using System.Collections.Generic;

namespace GarageGroup.Infra;

internal sealed partial class DataverseApiClient : IDataverseApiClient
{
    private const string ApiVersionData = "9.2";

    private const string ApiTypeData = "data";

    private const string WhoAmIRelativeUrl = "WhoAmI";

    private const string BatchRelativeUrl = "$batch";

    private const string AcceptHeaderName = "Accept";

    private const string CallerIdHeaderName = "MSCRMCallerID";

    private const string CallerObjectIdHeaderName = "CallerObjectId";

    private const string PreferHeaderName = "Prefer";

    private const string ReturnRepresentationValue = "return=representation";

    private const string SuppressDuplicateDetectionHeaderName = "MSCRM.SuppressDuplicateDetection";

    private const string IfMatchHeaderName = "If-Match";

    private const string IfMatchHeaderValueAll = "*";

    private const string SearchRequestUrl = "/api/search/v1.0/query";

    private const string PagingCookieAttributeName = "pagingcookie";

    private readonly IDataverseHttpApi httpApi;

    private readonly IGuidProvider guidProvider;

    private readonly Guid? callerId;

    internal DataverseApiClient(IDataverseHttpApi httpApi, IGuidProvider guidProvider)
    {
        this.httpApi = httpApi;
        this.guidProvider = guidProvider;
    }

    private DataverseApiClient(IDataverseHttpApi httpApi, IGuidProvider guidProvider, Guid callerId)
    {
        this.httpApi = httpApi;
        this.guidProvider = guidProvider;
        this.callerId = callerId;
    }

    private static string BuildDataRequestUrl(string dataUrl)
        =>
        $"/api/{ApiTypeData}/v{ApiVersionData}/{dataUrl}";

    private IEnumerable<DataverseHttpHeader> GetAllHeadersWithRepresentation(
        Guid? callerObjectId, bool? suppressDuplicateDetection, bool? isUpsert = null)
        =>
        GetAllHeaders(
            callerObjectId,
            new(PreferHeaderName, ReturnRepresentationValue),
            GetSuppressDuplicateDetectionHeader(suppressDuplicateDetection),
            GetIfMatchHeader(isUpsert));

    private IEnumerable<DataverseHttpHeader> GetAllHeadersWithoutRepresentation(
        Guid? callerObjectId, bool? suppressDuplicateDetection, bool? isUpsert = null)
        =>
        GetAllHeaders(
            callerObjectId,
            GetSuppressDuplicateDetectionHeader(suppressDuplicateDetection),
            GetIfMatchHeader(isUpsert));

    private IEnumerable<DataverseHttpHeader> GetAllHeaders(Guid? callerObjectId, params DataverseHttpHeader?[] headers)
    {
        var callerIdHeader = GetCallerObjectIdHeader(callerObjectId) ?? GetCallerIdHeader(callerId);
        if (callerIdHeader is not null)
        {
            yield return callerIdHeader;
        }

        foreach (var header in headers)
        {
            if (header is null)
            {
                continue;
            }

            yield return header;
        }
    }

    private FlatArray<DataverseHttpHeader> GetAllHeaders(Guid? callerObjectId = null)
    {
        var callerIdHeader = GetCallerObjectIdHeader(callerObjectId) ?? GetCallerIdHeader(callerId);
        return callerIdHeader is null ? default : new(callerIdHeader);
    }

    private static DataverseHttpHeader? GetCallerIdHeader(Guid? callerId)
        =>
        callerId is null ? null : new(CallerIdHeaderName, callerId.Value.ToString("D"));

    private static DataverseHttpHeader? GetCallerObjectIdHeader(Guid? callerObjectId)
        =>
        callerObjectId is null ? null : new(CallerObjectIdHeaderName, callerObjectId.Value.ToString("D"));

    private static DataverseHttpHeader? GetSuppressDuplicateDetectionHeader(bool? suppressDuplicateDetection)
    {
        if (suppressDuplicateDetection is null)
        {
            return null;
        }

        return new(SuppressDuplicateDetectionHeaderName, suppressDuplicateDetection.Value ? "true" : "false");
    }

    private static DataverseHttpHeader? GetIfMatchHeader(bool? isUpsert)
    {
        if (isUpsert is not false)
        {
            return null;
        }

        return new(IfMatchHeaderName, IfMatchHeaderValueAll);
    }

    private static DataverseHttpHeader? BuildPreferHeader(string? includeAnnotations, int? maxPageSize = null)
    {
        var value = string.Join(',', GetPreferValues());
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        return new(PreferHeaderName, value);

        IEnumerable<string> GetPreferValues()
        {
            if (maxPageSize is not null)
            {
                yield return $"odata.maxpagesize={maxPageSize}";
            }

            if (string.IsNullOrEmpty(includeAnnotations) is false)
            {
                yield return $"odata.include-annotations={includeAnnotations}";
            }
        }
    }

    private static Failure<DataverseFailureCode> ToDataverseFailure(Exception exception, string message)
        =>
        new(DataverseFailureCode.Unknown, message)
        {
            SourceException = exception
        };
}