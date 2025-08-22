using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    [Fact]
    public static async Task ExecuteChangeSetAsync_InputIsEmpty_ExpectHttpApiCalledNever()
    {
        var mockHttpApi = CreateMockChangeSetHttpApi(SomeChangeSetResponse);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        _ = await dataverseApiClient.ExecuteChangeSetAsync<StubRequestJson>(default, default);

        mockHttpApi.Verify(
            a => a.SendChangeSetAsync(It.IsAny<DataverseChangeSetRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public static async Task ExecuteChangeSetAsync_InputIsEmpty_ExpectSuccessEmpty()
    {
        var mockHttpApi = CreateMockChangeSetHttpApi(SomeChangeSetResponse);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var actual = await dataverseApiClient.ExecuteChangeSetAsync<StubRequestJson>(default, default);
        var expected = default(Unit);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task ExecuteChangeSetAsync_InputContainsInvalidRequestType_ExpectUnknownFailure()
    {
        var mockHttpApi = CreateMockChangeSetHttpApi(SomeChangeSetResponse);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = new DataverseChangeSetExecuteIn<object>(
            requests:
            [
                new StubTransactableIn<object>(),
                SomeDataverseEntityDeleteInput
            ]);

        var actual = await dataverseApiClient.ExecuteChangeSetAsync(input, default);
        Assert.True(actual.IsFailure);

        var actualFailure = actual.FailureOrThrow();
        Assert.StrictEqual(DataverseFailureCode.Unknown, actualFailure.FailureCode);

        var actualExceptionMessage = actualFailure.SourceException?.Message;
        var expectedExceptionMessage = $"An unexpected change set type: '{typeof(StubTransactableIn<object>).FullName}'";

        Assert.Equal(expectedExceptionMessage, actualExceptionMessage);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.ChangeSetExecuteInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task ExecuteChangeSetAsync_InputDoesNotContainInvalidRequestType_ExpectHttpRequestCalledOnce(
        Guid? callerId, Guid batchId, Guid changeSetId, DataverseChangeSetExecuteIn<object> input, DataverseChangeSetRequest expectedRequest)
    {
        var mockHttpApi = CreateMockChangeSetHttpApi(SomeChangeSetResponse);
        var guidProvider = CreateGuidProvider(batchId, changeSetId);

        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, guidProvider, callerId);

        var token = new CancellationToken(canceled: false);
        _ = await dataverseApiClient.ExecuteChangeSetAsync(input, token);

        mockHttpApi.Verify(p => p.SendChangeSetAsync(expectedRequest, token), Times.Once);
    }

    [Fact]
    public static async Task ExecuteChangeSetAsync_HttpApiThrowsException_ExpectFailure()
    {
        var sourceException = new Exception("Some exception message");

        var mockHttpApi = CreateMockHttpApi(sourceException);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeChangeSetInput;
        var actual = await dataverseApiClient.ExecuteChangeSetAsync(input, default);

        var expected = Failure.Create(
            DataverseFailureCode.Unknown,
            "An unexpected exception was thrown when trying to execute a Dataverse change set",
            sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.FailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static async Task ExecuteChangeSetAsync_ResponseIsFailure_ExpectFailure(
        Failure<DataverseFailureCode> failure)
    {
        var mockHttpApi = CreateMockChangeSetHttpApi(failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeChangeSetInput;
        var actual = await dataverseApiClient.ExecuteChangeSetAsync(input, default);

        Assert.StrictEqual(failure, actual);
    }

    [Fact]
    public static async Task ExecuteChangeSetAsync_ResponseIsSuccess_ExpectSuccess()
    {
        var mockHttpApi = CreateMockChangeSetHttpApi(SomeChangeSetResponse);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object, CreateGuidProvider());

        var input = SomeChangeSetInput;

        var actual = await dataverseApiClient.ExecuteChangeSetAsync(input, default);
        var expected = default(Unit);

        Assert.StrictEqual(expected, actual);
    }
}