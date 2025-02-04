using System;
using System.Net;
using System.Threading;
using CateringBackend.AuthUtilities;
using CateringBackend.Controllers;
using CateringBackend.Users.Client.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ClientControllerTests
{
    public class EditClientDetailsTests
    {
        private readonly ClientController _clientController;
        private readonly Mock<IMediator> _mockedMediator;
        private readonly Mock<IUserIdFromTokenProvider> _mockedUserIdFromTokenProvider;

        public EditClientDetailsTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mockedUserIdFromTokenProvider = new Mock<IUserIdFromTokenProvider>();
            _clientController = new ClientController(_mockedMediator.Object, _mockedUserIdFromTokenProvider.Object);
        }

        [Fact]
        public async void WhenEditClient_ThenGetUserIdFromContextOrThrowIsExecuted()
        {
            // Arrange
            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Verifiable("GetUserIdFromContextOrThrow not executed");

            // Act
            await _clientController.EditClient(new EditClientCommand());

            // Assert
            _mockedUserIdFromTokenProvider.Verify(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()),Times.Once);
        }

        [Fact]
        public async void GivenClientIdFromService_WhenEditClient_ThenCommandWithIdIsSent()
        {
            // Arrange
            var clientIdReturnedByService = Guid.NewGuid();

            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Returns(clientIdReturnedByService);

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<EditClientWithIdCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("EditClientWithIdCommand was not sent");

            var editClientCommand = new EditClientCommand();

            // Act 
            await _clientController.EditClient(editClientCommand);

            // Assert
            _mockedMediator
                .Verify(
                    x => x.Send(It.IsAny<EditClientWithIdCommand>(), It.IsAny<CancellationToken>()), 
                    Times.Once);
            _mockedMediator
                .Verify(
                    x => x.Send(It.Is<EditClientWithIdCommand>(x => x.ClientId == clientIdReturnedByService), It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Theory]
        [InlineData(false, HttpStatusCode.BadRequest)]
        [InlineData(true, HttpStatusCode.OK)]
        public async void GivenValueReturnedByMediator_WhenEditClient_ThenReturnProperStatusCode(bool mediatorReturnedValue, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<EditClientWithIdCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mediatorReturnedValue);

            // Act 
            var result = await _clientController.EditClient(new EditClientCommand());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Arrange
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
