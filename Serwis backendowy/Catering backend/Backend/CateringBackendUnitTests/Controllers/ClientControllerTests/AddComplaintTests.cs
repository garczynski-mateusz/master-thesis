using CateringBackend.AuthUtilities;
using CateringBackend.Controllers;
using CateringBackend.Users.Client.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ClientControllerTests
{
    public class AddComplaintTests
    {
        private readonly ClientController _clientController;
        private readonly Mock<IMediator> _mockedMediator;
        private readonly Mock<IUserIdFromTokenProvider> _mockedUserIdFromTokenProvider;

        public AddComplaintTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mockedUserIdFromTokenProvider = new Mock<IUserIdFromTokenProvider>();
            _clientController = new ClientController(_mockedMediator.Object, _mockedUserIdFromTokenProvider.Object);
        }

        [Fact]
        public async void WhenAddComplaint_ThenGetUserIdFromContextOrThrowIsExecuted()
        {
            // Arrange
            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Verifiable("GetUserIdFromContextOrThrow not executed");

            // Act
            await _clientController.AddComplaint(Guid.NewGuid(), new AddComplaintCommand());

            // Assert
            _mockedUserIdFromTokenProvider.Verify(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async void GivenAddComplaintCommandAndIdFromService_WhenAddComplaint_ThenCommandWithIdIsSent()
        {
            // Arrange
            var clientIdReturnedByService = Guid.NewGuid();

            _mockedUserIdFromTokenProvider
                .Setup(x => x.GetUserIdFromContextOrThrow(It.IsAny<HttpContext>()))
                .Returns(clientIdReturnedByService);

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AddComplaintCommandWithClientId>(), It.IsAny<CancellationToken>()))
                .Verifiable("AddOrderCommandWithClientId was not sent");

            var orderId = Guid.NewGuid();

            // Act
            await _clientController.AddComplaint(orderId, new AddComplaintCommand());

            // Assert
            _mockedMediator
                .Verify(
                    x => x.Send(It.IsAny<AddComplaintCommandWithClientId>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            _mockedMediator
                .Verify(
                    x => x.Send(It.Is<AddComplaintCommandWithClientId>(x => x.ClientId == clientIdReturnedByService && x.OrderId == orderId),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(false, false, HttpStatusCode.NotFound)]
        [InlineData(true, false, HttpStatusCode.BadRequest)]
        [InlineData(true, true, HttpStatusCode.Created)]
        public async void GivenValueReturnedByMediator_WhenAddComplaint_ThenReturnProperHttpStatusCode(
            bool orderExists, bool complaintAdded, HttpStatusCode expectedHttpStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AddComplaintCommandWithClientId>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((orderExists, complaintAdded)))
                .Verifiable("Add complaint command was not sent");

            // Act
            var result = await _clientController.AddComplaint(Guid.NewGuid(), new AddComplaintCommand());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Arrange
            Assert.Equal((int)expectedHttpStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
