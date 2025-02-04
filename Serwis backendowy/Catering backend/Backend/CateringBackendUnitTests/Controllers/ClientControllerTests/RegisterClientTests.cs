using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.AuthUtilities;
using CateringBackend.Controllers;
using CateringBackend.Users.Client.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ClientControllerTests
{
    public class RegisterClientTests
    {
        private readonly ClientController _clientController;
        private readonly Mock<IMediator> _mockedMediator;
        private readonly Mock<IUserIdFromTokenProvider> _mockedUserIdFromTokenProvider;

        public RegisterClientTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mockedUserIdFromTokenProvider = new Mock<IUserIdFromTokenProvider>();
            _clientController = new ClientController(_mockedMediator.Object, _mockedUserIdFromTokenProvider.Object);
        }

        [Fact]
        public async void GivenRegisterClientCommand_WhenRegisterClient_ThenItIsSendToMediator()
        {
            // Arrange
            var registerClientCommand = new RegisterClientCommand();

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<RegisterClientCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("Register client query was not sent");

            // Act 
            await _clientController.RegisterClient(registerClientCommand);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<RegisterClientCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("not empty string as token", HttpStatusCode.Created)]
        [InlineData("", HttpStatusCode.BadRequest)]
        [InlineData("   ", HttpStatusCode.BadRequest)]
        [InlineData(null, HttpStatusCode.BadRequest)]
        public async void GivenMediatorResult_WhenRegisterClient_ThenReturnsProperStatusCode(string mediatorResult, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<RegisterClientCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mediatorResult))
                .Verifiable("Register client query was not sent");

            // Act 
            var result = await _clientController.RegisterClient(new RegisterClientCommand());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
