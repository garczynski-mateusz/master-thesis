using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.AuthUtilities;
using CateringBackend.Controllers;
using CateringBackend.Users.Client.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ClientControllerTests
{
    public class LoginClientTests
    {
        private readonly ClientController _clientController;
        private readonly Mock<IMediator> _mockedMediator;
        private readonly Mock<IUserIdFromTokenProvider> _mockedUserIdFromTokenProvider;

        public LoginClientTests()
        {
            _mockedUserIdFromTokenProvider = new Mock<IUserIdFromTokenProvider>();
            _mockedMediator = new Mock<IMediator>();
            _clientController = new ClientController(_mockedMediator.Object, _mockedUserIdFromTokenProvider.Object);
        }

        [Fact]
        public async void GivenLoginClientQuery_WhenLoginUser_ThenItIsSentToMediator()
        {
            // Arrange
            var loginQuery = new LoginClientQuery
            {
                Email = "testEmail",
                Password = "testPassword"
            };

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<LoginClientQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("Login client query was not sent");

            // Act 
            await _clientController.LoginClient(loginQuery);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<LoginClientQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("not empty string as token", HttpStatusCode.OK)]
        [InlineData("", HttpStatusCode.BadRequest)]
        [InlineData("   ", HttpStatusCode.BadRequest)]
        [InlineData(null, HttpStatusCode.BadRequest)]
        public async void GivenMediatorResult_WhenLoginUser_ThenReturnsProperStatusCode(string mediatorResult, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<LoginClientQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mediatorResult))
                .Verifiable("Client login query was not sent");

            // Act 
            var result = await _clientController.LoginClient(new LoginClientQuery());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
