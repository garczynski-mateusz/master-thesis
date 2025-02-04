using CateringBackend.AuthUtilities;
using CateringBackend.Controllers;
using CateringBackend.Users.Client.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ClientControllerTests
{
    public class GetClientOrdersTests
    {
        private readonly ClientController _clientController;
        private readonly Mock<IMediator> _mockedMediator;
        private readonly Mock<IUserIdFromTokenProvider> _mockedUserIdFromTokenProvider;

        public GetClientOrdersTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mockedUserIdFromTokenProvider = new Mock<IUserIdFromTokenProvider>();
            _clientController = new ClientController(_mockedMediator.Object, _mockedUserIdFromTokenProvider.Object);
        }

        [Fact]
        public async void WhenGetClientOrders_ThenGetUserIdFromContextOrThrowIsExecuted()
        {
            // Arrange
            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Verifiable("GetUserIdFromContextOrThrow not executed");

            // Act
            await _clientController.GetOrders(new GetOrdersQuery());

            // Assert
            _mockedUserIdFromTokenProvider.Verify(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async void WhenGetClientOrders_ThenGetClietnOrdersQueryWithProperIdIsSentToMediator()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Returns(userId);

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetOrdersQueryWithUserId>(), It.IsAny<CancellationToken>()))
                .Verifiable("Get client orders query was not sent");

            // Act 
            await _clientController.GetOrders(new GetOrdersQuery());

            // Assert
            _mockedMediator
                .Verify(
                    x => x.Send(It.IsAny<GetOrdersQueryWithUserId>(), It.IsAny<CancellationToken>()),
                    Times.Once);
            _mockedMediator
                .Verify(
                    x => x.Send(It.Is<GetOrdersQueryWithUserId>(x => x.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void GivenMediatorResultAsNull_WhenGetClientOrders_ThenReturnsBadRequest()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetOrdersQueryWithUserId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<OrderDTO>)null);

            // Act
            var result = await _clientController.GetOrders(new GetOrdersQuery());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCodeActionResult?.StatusCode);
        }

        [Fact]
        public async void GivenMediatorResultWhichIsNotNull_WhenGetClientOrders_ThenReturnsOkResult()
        {
            // Arrange
            var mockedOrdersDto = new List<OrderDTO>() { new OrderDTO() };

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetOrdersQueryWithUserId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockedOrdersDto);

            // Act
            var result = await _clientController.GetOrders(new GetOrdersQuery());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, statusCodeActionResult?.StatusCode);
        }
    }
}
