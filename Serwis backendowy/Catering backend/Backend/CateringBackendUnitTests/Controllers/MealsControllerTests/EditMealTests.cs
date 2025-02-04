using System;
using System.Net;
using System.Threading;
using CateringBackend.Controllers;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.MealsControllerTests
{
    public class EditMealTests
    {
        private readonly MealsController _mealsController;
        private readonly Mock<IMediator> _mockedMediator;

        public EditMealTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mealsController = new MealsController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenEditMealCommand_WhenEditMeal_ThenItIsSentToMediatorWithProperMealId()
        {
            // Arrange
            var editMealCommand = new EditMealCommand
            {
                MealId = Guid.NewGuid()
            };
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<EditMealCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("EditMealCommand was not sent");

            // Act
            await _mealsController.EditMeal(editMealCommand.MealId, editMealCommand);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<EditMealCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockedMediator.Verify(x => x.Send(It.Is<EditMealCommand>(x => x.MealId == editMealCommand.MealId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(false, false, HttpStatusCode.NotFound)]
        [InlineData(true, false, HttpStatusCode.BadRequest)]
        [InlineData(true, true, HttpStatusCode.OK)]
        public async void GivenValueReturnedByMediator_WhenEditMeal_ThenReturnProperStatusCode(
            bool returnMealExists, bool returnMealEdited, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var editMealCommand = new EditMealCommand
            {
                MealId = Guid.NewGuid()
            };
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<EditMealCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((returnMealExists, returnMealEdited));

            // Act
            var result = await _mealsController.EditMeal(editMealCommand.MealId, editMealCommand);
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}