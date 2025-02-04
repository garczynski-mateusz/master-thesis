using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Controllers;
using CateringBackend.Users.Deliverer.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.DelivererControllerTests
{
    public class LoginDelivererTests
    {
        private readonly DelivererController _delivererController;
        private readonly Mock<IMediator> _mockedMediator;

        public LoginDelivererTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _delivererController = new DelivererController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenLoginDelivererQuery_WhenLoginDeliverer_ThenItIsSentToMediator()
        {
            // Arrange
            var loginQuery = new LoginDelivererQuery
            {
                Email = "testEmail",
                Password = "testPassword"
            };

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<LoginDelivererQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("Login deliverer query was not sent");

            // Act 
            await _delivererController.LoginDeliverer(loginQuery);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<LoginDelivererQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("not empty string as token", HttpStatusCode.OK)]
        [InlineData("", HttpStatusCode.BadRequest)]
        [InlineData("   ", HttpStatusCode.BadRequest)]
        [InlineData(null, HttpStatusCode.BadRequest)]
        public async void GivenMediatorResult_WhenLoginDeliverer_ThenReturnsProperStatusCode(string mediatorResult, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<LoginDelivererQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mediatorResult))
                .Verifiable("Login deliverer query was not sent");

            // Act 
            var result = await _delivererController.LoginDeliverer(new LoginDelivererQuery());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
