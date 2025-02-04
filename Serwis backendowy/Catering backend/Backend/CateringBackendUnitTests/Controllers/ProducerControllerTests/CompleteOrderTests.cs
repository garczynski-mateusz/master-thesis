using CateringBackend.Controllers;
using CateringBackend.Users.Producer.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ProducerControllerTests
{
    public class CompleteOrderTests
    {
        private readonly ProducerController _producersController;
        private readonly Mock<IMediator> _mockedMediator;

        public CompleteOrderTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _producersController = new ProducerController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenCompleteOrderCommand_WhenCompleteOrder_ThenItIsSentToMediator()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<CompleteOrderCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("Complete order command was not sent");

            // Act 
            await _producersController.CompleteOrder(Guid.NewGuid());

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<CompleteOrderCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(false, false, HttpStatusCode.NotFound)]
        [InlineData(true, false, HttpStatusCode.BadRequest)]
        [InlineData(true, true, HttpStatusCode.OK)]
        public async void GivenMediatorReturnValue_WhenCompleteOrder_ThenReturnsProperHttpStatusCode(
            bool orderExists, bool orderCompleted, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<CompleteOrderCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((orderExists, orderCompleted)))
                .Verifiable("Complete order command was not sent");

            // Act 
            var result = await _producersController.CompleteOrder(Guid.NewGuid());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
