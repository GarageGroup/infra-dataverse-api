﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace GGroupp.Infra.Dataverse.Api.Test;

partial class DataverseApiClientTest
{
    
    private static Mock<IDataverseHttpApi> CreateMockHttpApiSendEmail(
        Result<DataverseEmailCreateJsonOut, Failure<DataverseFailureCode>> creationResult,
        Result<Unit, Failure<DataverseFailureCode>> sendingResult)
    {
        var mock = new Mock<IDataverseHttpApi>();

        _ = mock
            .Setup(
                p => p.InvokeAsync<DataverseEmailCreateJsonIn, DataverseEmailCreateJsonOut>(
                    It.IsAny<DataverseHttpRequest<DataverseEmailCreateJsonIn>>(), It.IsAny<CancellationToken>()))!
            .ReturnsAsync(creationResult);

        _ = mock
            .Setup(
                p => p.InvokeAsync<DataverseEmailSendJsonIn, Unit>(
                    It.IsAny<DataverseHttpRequest<DataverseEmailSendJsonIn>>(), It.IsAny<CancellationToken>()))!
            .ReturnsAsync(sendingResult);
        
        return mock;
    }
    
    [Fact]
    internal static async Task SendEmailAsync_InputIsNull_ExpectArgumentNullException()
    {
        // Arrange
        var mockHttpApi = CreateMockHttpApiSendEmail(SomeEmailCreateJsonOut, default(Unit));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object);

        var token = new CancellationToken(canceled: false);
        
        // Act
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(InnerSendEmailAsync);

        // Assert
        Assert.Equal("input", ex.ParamName);

        Task InnerSendEmailAsync()
            =>
            dataverseApiClient.SendEmailAsync(null!, token).AsTask();
    }

    [Fact]
    internal static void SendEmailAsync_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        // Arrange
        var mockHttpApi = CreateMockHttpApiSendEmail(SomeEmailCreateJsonOut, default(Unit));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object);

        var token = new CancellationToken(canceled: true);

        // Act
        var actualTask = dataverseApiClient.SendEmailAsync(
            SomeEmailSendInWithEmailId, token);

        // Assert
        Assert.True(actualTask.IsCanceled);
    }

    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.GetEmailSendInputTestData), MemberType = typeof(ApiClientTestDataSource))]
    internal static async Task SendEmailAsync_CancellationTokenIsNotCanceled_ExpectHttpRequestCalledOnce(
        DataverseEmailSendIn input,
        DataverseHttpRequest<DataverseEmailCreateJsonIn>? expectedCreationRequest,
        DataverseHttpRequest<DataverseEmailSendJsonIn> expectedSendingRequest,
        DataverseEmailCreateJsonOut? createOut)
    {
        // Arrange
        var mockHttpApi = CreateMockHttpApiSendEmail(createOut ?? SomeEmailCreateJson, default(Unit));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object);

        var token = new CancellationToken(canceled: false);
        
        // Act
        _ = await dataverseApiClient.SendEmailAsync(input, token);
        
        //Assert
        if (expectedCreationRequest is not null)
        {
            mockHttpApi.Verify(
                p => p.InvokeAsync<DataverseEmailCreateJsonIn, DataverseEmailCreateJsonOut>(expectedCreationRequest.Value, token), Times.Once);
        }
        
        mockHttpApi.Verify(
            p => p.InvokeAsync<DataverseEmailSendJsonIn, Unit>(expectedSendingRequest, token), Times.Once);
    }
    
    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.GetFailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static  async Task SendEmailAsync_ResponseIsFailure_ExpectFailureInEmailCreation(
        Failure<DataverseFailureCode> failure)
    {
        // Arrange
        var mockHttpApi = CreateMockHttpApiSendEmail(failure, default(Unit));
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object);
        

        // Act
        var actual = await dataverseApiClient.SendEmailAsync(SomeEmailSendInWithOutEmailId, CancellationToken.None);
        
        // Assert
        Assert.Equal(failure, actual);
    }
    
    [Theory]
    [MemberData(nameof(ApiClientTestDataSource.GetFailureOutputTestData), MemberType = typeof(ApiClientTestDataSource))]
    public static  async Task SendEmailAsync_ResponseIsFailure_ExpectFailureInEmailSending(
        Failure<DataverseFailureCode> failure)
    {
        // Arrange
        var mockHttpApi = CreateMockHttpApiSendEmail(SomeEmailCreateJson, failure);
        var dataverseApiClient = CreateDataverseApiClient(mockHttpApi.Object);
        

        // Act
        var actual = await dataverseApiClient.SendEmailAsync(SomeEmailSendInWithEmailId, CancellationToken.None);
        
        // Assert
        Assert.Equal(failure, actual);
    }
    
}
