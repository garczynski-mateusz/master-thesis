using CateringBackend.Controllers;
using CateringBackend.Users.Deliverer.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Controllers.DelivererControllerTests
{
    public class DeliverOrderTests
    {
        private readonly DelivererController _delivererController;
        private readonly Mock<IMediator> _mockedMediator;

        public DeliverOrderTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _delivererController = new DelivererController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenDeliverOrderCommand_WhenDeliverOrder_ThenItIsSentToMediatorWithProperId()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<DeliverOrderCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("Deliver order command was not sent");

            // Act 
            await _delivererController.DeliverOrder(orderId);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<DeliverOrderCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockedMediator.Verify(x => x.Send(It.Is<DeliverOrderCommand>(x => x.OrderId == orderId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(false, false, HttpStatusCode.NotFound)]
        [InlineData(true, false, HttpStatusCode.BadRequest)]
        [InlineData(true, true, HttpStatusCode.OK)]
        public async void GivenMediatorReturnValue_WhenAnswerComplaint_ThenReturnsProperHttpStatusCode(
            bool orderExists, bool orderDelivered, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<DeliverOrderCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((orderExists, orderDelivered)))
                .Verifiable("Answer complaint command was not sent");

            // Act 
            var result = await _delivererController.DeliverOrder(Guid.NewGuid());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}