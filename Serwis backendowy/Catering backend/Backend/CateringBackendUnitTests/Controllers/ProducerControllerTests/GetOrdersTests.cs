using CateringBackend.Controllers;
using CateringBackend.Users.Producer.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System.Net;
using System.Threading;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ProducerControllerTests
{
    public class GetOrdersTests
    {
        private readonly ProducerController _producersController;
        private readonly Mock<IMediator> _mockedMediator;

        public GetOrdersTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _producersController = new ProducerController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenGetOrdersQuery_WhenGetOrders_ThenItIsSentToMediator()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("Get orders query was not sent");

            // Act 
            await _producersController.GetOrders(new GetOrdersQuery());

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void GivenGetOrdersQuery_WhenGetOrders_ThenReturnsProperHttpStatusCode()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("Get orders query was not sent");

            // Act 
            var result = await _producersController.GetOrders(new GetOrdersQuery());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeActionResult.StatusCode);
        }
    }
}
