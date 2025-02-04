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
    public class DeleteMealTests
    {
        private readonly MealsController _mealsController;
        private readonly Mock<IMediator> _mockedMediator;

        public DeleteMealTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mealsController = new MealsController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenDeleteMealCommand_WhenDeleteMeal_ThenItIsSentToMediatorWithProperMealId()
        {
            // Arrange
            var mealId = Guid.NewGuid();
            var deleteMealCommand = new DeleteMealCommand(mealId);

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<DeleteMealCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("DeleteMealCommand was not sent");

            // Act
            await _mealsController.DeleteMeal(mealId);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<DeleteMealCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockedMediator.Verify(x => x.Send(It.Is<DeleteMealCommand>(x => x.MealId == mealId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [MemberData(nameof(DeleteMealTestsData.GetPossibleMediatorReturnsWithExpectedResultCode),
            MemberType = typeof(DeleteMealTestsData))]
        public async void GivenValueReturnedByMediator_WhenDeleteMeal_ThenReturnProperStatusCode(Meal mediatorReturnedValue, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<DeleteMealCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mediatorReturnedValue);

            // Act 
            var result = await _mealsController.DeleteMeal(Guid.NewGuid());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}