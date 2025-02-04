using CateringBackend.Controllers;
using CateringBackend.Diets.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Net;
using System.Threading;
using Xunit;

namespace CateringBackendUnitTests.Controllers.DietsControllerTests
{
    public class DeleteDietTests
    {
        private readonly DietsController _dietsController;
        private readonly Mock<IMediator> _mockedMediator;

        public DeleteDietTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _dietsController = new DietsController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenDeleteDietCommand_WhenDeleteDiet_ThenItIsSendToMediatorWithProperDietId()
        {
            // Arrange
            var dietId = Guid.NewGuid();
            var deleteDietCommand = new DeleteDietCommand(dietId);

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<DeleteDietCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("DeleteDietCommand was not sent");

            // Act
            await _dietsController.DeleteDiet(dietId);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<DeleteDietCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockedMediator.Verify(x => x.Send(It.Is<DeleteDietCommand>(x => x.DietId == dietId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(false, "", HttpStatusCode.NotFound)]
        [InlineData(true, "", HttpStatusCode.OK)]
        public async void GivenValueReturnedByMediator_WhenDeleteDiet_ThenReturnProperStatusCode(
            bool dietExists, string errorMessage, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<DeleteDietCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((dietExists, errorMessage));

            // Act 
            var result = await _dietsController.DeleteDiet(Guid.NewGuid());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
