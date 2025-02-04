using CateringBackend.Controllers;
using CateringBackend.Users;
using CateringBackend.Users.Client.Queries;
using CateringBackend.Users.Deliverer.Queries;
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
    public class GetDelivererOrdersTests
    {
        private readonly DelivererController _delivererController;
        private readonly Mock<IMediator> _mockedMediator;

        public GetDelivererOrdersTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _delivererController = new DelivererController(_mockedMediator.Object);
        }

        [Fact]
        public async void WhenGetDelivererOrders_ThenGetDelivererOrdersQueryWithProperIdIsSentToMediator()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetDelivererOrdersQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("Get deliverer orders query was not sent");

            // Act 
            await _delivererController.GetOrders();

            // Assert
            _mockedMediator
                .Verify(
                    x => x.Send(It.IsAny<GetDelivererOrdersQuery>(), It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async void GivenMediatorResultAsNull_WhenGetDelivererOrders_ThenReturnsBadRequest()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetDelivererOrdersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<DelivererOrdersQueryDTO>)null);

            // Act
            var result = await _delivererController.GetOrders();
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCodeActionResult?.StatusCode);
        }

        [Fact]
        public async void GivenMediatorResultWhichIsNotNull_WhenGetDelivererOrders_ThenReturnsOkResult()
        {
            // Arrange
            var mockedOrdersDto = new List<DelivererOrdersQueryDTO>() { new DelivererOrdersQueryDTO() };

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetDelivererOrdersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockedOrdersDto);

            // Act
            var result = await _delivererController.GetOrders();
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, statusCodeActionResult?.StatusCode);
        }
    }
}
