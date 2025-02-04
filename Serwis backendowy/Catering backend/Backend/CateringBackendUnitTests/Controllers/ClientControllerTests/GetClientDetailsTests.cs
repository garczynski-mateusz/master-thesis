using System;
using System.Net;
using System.Threading;
using CateringBackend.AuthUtilities;
using CateringBackend.Controllers;
using CateringBackend.Domain.Entities;
using CateringBackend.Users.Client.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ClientControllerTests
{
    public class GetClientDetailsTests
    {
        private readonly ClientController _clientController;
        private readonly Mock<IMediator> _mockedMediator;
        private readonly Mock<IUserIdFromTokenProvider> _mockedUserIdFromTokenProvider;

        public GetClientDetailsTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mockedUserIdFromTokenProvider = new Mock<IUserIdFromTokenProvider>();
            _clientController = new ClientController(_mockedMediator.Object, _mockedUserIdFromTokenProvider.Object);
        }

        [Fact]
        public async void WhenGetClientDetails_ThenGetUserIdFromContextOrThrowIsExecuted()
        {
            // Arrange
            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Verifiable("GetUserIdFromContextOrThrow not executed");

            // Act
            await _clientController.GetClientDetails();

            // Assert
            _mockedUserIdFromTokenProvider.Verify(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()),Times.Once);
        }
        
        [Fact]
        public async void WhenGetClientDetails_ThenGetClientDetailsQueryWithProperIdIsSentToMediator()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Returns(userId);

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetClientDetailsQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("Get client details query was not sent");

            // Act 
            await _clientController.GetClientDetails();

            // Assert
            _mockedMediator
                .Verify(
                    x => x.Send(It.IsAny<GetClientDetailsQuery>(), It.IsAny<CancellationToken>()), 
                    Times.Once);
            _mockedMediator
                .Verify(
                    x => x.Send(It.Is<GetClientDetailsQuery>(x => x.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void GivenMediatorResultAsNull_WhenGetClientDetails_ThenReturnsNotFound()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetClientDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClientDetailsDto)null);

            // Act
            var result = await _clientController.GetClientDetails();
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, statusCodeActionResult?.StatusCode);
        }

        [Fact]
        public async void GivenMediatorResultWhichIsNotNull_WhenGetClientDetails_ThenReturnsOkResult()
        {
            // Arrange
            var mockedClientDetailsDto = new ClientDetailsDto(new Client(), new Address());
            
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetClientDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockedClientDetailsDto);

            // Act
            var result = await _clientController.GetClientDetails();
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, statusCodeActionResult?.StatusCode);
        }
    }
}
