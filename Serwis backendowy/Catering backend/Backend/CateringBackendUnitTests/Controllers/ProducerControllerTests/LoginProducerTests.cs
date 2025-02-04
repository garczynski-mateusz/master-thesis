using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Controllers;
using CateringBackend.Users.Producer.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ProducerControllerTests
{
    public class LoginProducerTests
    {
        private readonly ProducerController _producersController;
        private readonly Mock<IMediator> _mockedMediator;

        public LoginProducerTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _producersController = new ProducerController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenLoginProducerQuery_WhenLoginProducer_ThenItIsSentToMediator()
        {
            // Arrange
            var loginQuery = new LoginProducerQuery()
            {
                Email = "testEmail",
                Password = "testPassword"
            };

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<LoginProducerQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("Login client query was not sent");

            // Act 
            await _producersController.LoginProducer(loginQuery);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<LoginProducerQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("not empty string as token", HttpStatusCode.OK)]
        [InlineData("", HttpStatusCode.BadRequest)]
        [InlineData("   ", HttpStatusCode.BadRequest)]
        [InlineData(null, HttpStatusCode.BadRequest)]
        public async void GivenMediatorResult_WhenLoginProducer_ThenReturnsProperStatusCode(string mediatorResult, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<LoginProducerQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mediatorResult))
                .Verifiable("Client login query was not sent");

            // Act 
            var result = await _producersController.LoginProducer(new LoginProducerQuery());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
