using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.WhoAmIInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task WhoAmIAsync_ExpectHttpRequestCalledOnce(
        Guid? callerId, DataverseWhoAmIIn input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeWhoAmIOutJson.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider(), callerId);

        _ = await dataverseApiClient.WhoAmIAsync(input, TestContext.Current.CancellationToken);

        mockHttpApi.Verify(p => p.SendJsonAsync(expectedRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public static async Task WhoAmIAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new Exception("Some exception message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.WhoAmIAsync(default(DataverseWhoAmIIn), TestContext.Current.CancellationToken);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to get a Dataverse current user data",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task WhoAmIAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.WhoAmIAsync(default(DataverseWhoAmIIn), TestContext.Current.CancellationToken);
        Assert.StrictEqual(failure, actual);
    }

    [Fact]
    public static async Task WhoAmIAsync_ResponseIsSuccess_ExpectSuccess()
    {
        var success = new DataverseWhoAmIOutJson
        {
            BusinessUnitId = new("51ea96d6-5119-4059-b649-d90c0a4aeab6"),
            UserId = new("6ac49276-357d-441f-89c4-c118cbaa5ee3"),
            OrganizationId = new("92847989-5772-4ff9-851b-661dc66720ae")
        };

        var mockHttpApi = CreateMockJsonHttpApi(success.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.WhoAmIAsync(default(DataverseWhoAmIIn), TestContext.Current.CancellationToken);

        var expected = new DataverseWhoAmIOut(
            businessUnitId: new("51ea96d6-5119-4059-b649-d90c0a4aeab6"),
            userId: new("6ac49276-357d-441f-89c4-c118cbaa5ee3"),
            organizationId: new("92847989-5772-4ff9-851b-661dc66720ae"));

        Assert.StrictEqual(expected, actual);
    }
}