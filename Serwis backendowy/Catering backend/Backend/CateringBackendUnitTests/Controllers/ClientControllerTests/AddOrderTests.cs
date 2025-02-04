using CateringBackend.AuthUtilities;
using CateringBackend.Controllers;
using CateringBackend.Users.Client.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Net;
using System.Threading;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ClientControllerTests
{
    public class AddOrderTests
    {
        private readonly ClientController _clientController;
        private readonly Mock<IMediator> _mockedMediator;
        private readonly Mock<IUserIdFromTokenProvider> _mockedUserIdFromTokenProvider;

        public AddOrderTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mockedUserIdFromTokenProvider = new Mock<IUserIdFromTokenProvider>();
            _clientController = new ClientController(_mockedMediator.Object, _mockedUserIdFromTokenProvider.Object);
        }

        [Fact]
        public async void WhenAddOrder_ThenGetUserIdFromContextOrThrowIsExecuted()
        {
            // Arrange
            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Verifiable("GetUserIdFromContextOrThrow not executed");

            // Act
            await _clientController.AddOrder(new AddOrderCommand());

            // Assert
            _mockedUserIdFromTokenProvider.Verify(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()), Times.Once);
        }
    
        [Fact]
        public async void GivenAddOrderCommandAndIdFromService_WhenAddOrder_ThenCommandWithIdIsSent()
        {
            // Arrange
            var clientIdReturnedByService = Guid.NewGuid();

            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Returns(clientIdReturnedByService);

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AddOrderCommandWithClientId>(), It.IsAny<CancellationToken>()))
                .Verifiable("AddOrderCommandWithClientId was not sent");

            var addOrderCommand = new AddOrderCommand();

            // Act
            await _clientController.AddOrder(addOrderCommand);

            // Assert
            _mockedMediator
                .Verify(
                    x => x.Send(It.IsAny<AddOrderCommandWithClientId>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            _mockedMediator
                .Verify(
                    x => x.Send(It.Is<AddOrderCommandWithClientId>(x => x.ClientId == clientIdReturnedByService), It.IsAny<CancellationToken>()),
                    Times.Once);

        }
    
        [Theory]
        [InlineData(null, HttpStatusCode.BadRequest)]
        [InlineData("", HttpStatusCode.BadRequest)]
        [InlineData("Id of added order", HttpStatusCode.Created)]
        public async void GivenValueReturnedByMediator_WhenAddOrder_ThenReturnProperHttpStatusCode(
            string mediatorResult, HttpStatusCode expectedHttpStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AddOrderCommandWithClientId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mediatorResult);

            // Act
            var result = await _clientController.AddOrder(new AddOrderCommand());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Arrange
            Assert.Equal((int)expectedHttpStatusCode, statusCodeActionResult.StatusCode);
        }
    
        [Fact]
        public async void GivenValueReturnedByMediator_WhenAddOrder_ThenReturnAddedOrderIdInResposne()
        {
            // Arrange
            var addedOrderId = Guid.NewGuid().ToString();
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AddOrderCommandWithClientId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedOrderId);

            // Act
            var result = await _clientController.AddOrder(new AddOrderCommand());
            var objectResult = result as ObjectResult;

            // Assert
            Assert.Equal(addedOrderId, objectResult.Value.ToString());
        }
    }
}
