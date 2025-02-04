using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Controllers;
using CateringBackend.Meals;
using CateringBackend.Meals.Commands;
using CateringBackend.Meals.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.MealsControllerTests
{
    public class AddMealTests
    {
        private readonly MealsController _mealsController;
        private readonly Mock<IMediator> _mockedMediator;

        public AddMealTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mealsController = new MealsController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenAddMealCommand_WhenAddNewMeal_ThenItIsSentToMediator()
        {
            // Arrange
            var addMealCommand = new AddMealCommand();

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AddMealCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("AddMealCommand was not sent");

            // Act
            await _mealsController.AddNewMeal(addMealCommand);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<AddMealCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(false, HttpStatusCode.BadRequest)]
        [InlineData(true, HttpStatusCode.Created)]
        public async void GivenValueReturnedByMediator_WhenAddNewMeal_ThenReturnProperStatusCode(bool mediatorReturnedValue, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AddMealCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mediatorReturnedValue);

            // Act 
            var result = await _mealsController.AddNewMeal(new AddMealCommand());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
