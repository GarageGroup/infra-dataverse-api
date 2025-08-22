using System;
using System.Threading;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task CreateEmailAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeEmailCreateJsonOut.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var token = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerCreateEmailAsync);

        Assert.Equal("input", ex.ParamName);

        Task InnerCreateEmailAsync()
            =>
            dataverseApiClient.CreateEmailAsync(null!, token).AsTask();
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.EmailCreateInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task CreateEmailAsync_InputIsNotNull_ExpectHttpRequestCalledOnce(
        DataverseEmailCreateIn input, DataverseJsonRequest expectedRequest)
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeEmailCreateJsonOut.InnerToJsonResponse(), OnRequest);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var token = new CancellationToken(canceled: false);
        _ = await dataverseApiClient.CreateEmailAsync(input, token);

        mockHttpApi.Verify(p => p.SendJsonAsync(It.IsAny<DataverseJsonRequest>(), token), Times.Once);

        void OnRequest(DataverseJsonRequest actual)
            =>
            actual.Content.ShouldDeepEqual(expectedRequest.Content);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.InvalidEmailCreateInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task CreateEmailAsync_InputIsInvalid_ExpectFailure(
        DataverseEmailCreateIn invalidInput, Failure<DataverseFailureCode> expectedFailure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(SomeEmailCreateJsonOut.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var token = new CancellationToken(canceled: false);
        var result = await dataverseApiClient.CreateEmailAsync(invalidInput, token);

        Assert.StrictEqual(expectedFailure, result);
    }

    [Fact]
    public static async Task CreateEmailAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new InvalidOperationException("Some exception message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.CreateEmailAsync(SomeEmailCreateIn, default);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to create a Dataverse email",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task CreateEmailAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockJsonHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.CreateEmailAsync(SomeEmailCreateIn, default);

        Assert.StrictEqual(failure, actual);
    }

    [Fact]
    public static async Task CreateEmailAsync_ResponseIsSuccess_ExpectSuccess()
    {
        var emailOut = new DataverseEmailCreateJsonOut
        {
            ActivityId = new("2deae2b9-84e7-4316-9271-c74d60b56631")
        };

        var mockHttpApi = CreateMockJsonHttpApi(emailOut.InnerToJsonResponse());
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.CreateEmailAsync(SomeEmailCreateIn, default);

        var expected = new DataverseEmailCreateOut(
            emailId: new("2deae2b9-84e7-4316-9271-c74d60b56631"));

        Assert.StrictEqual(expected, actual);
    }
}