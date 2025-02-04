using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Controllers;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals;
using CateringBackend.Meals.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.MealsControllerTests
{
    public class GetMealDetailsTests
    {
        private readonly MealsController _mealsController;
        private readonly Mock<IMediator> _mockedMediator;

        public GetMealDetailsTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mealsController = new MealsController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenMealId_WhenGetMealDetails_ThenItIsSentToMediator()
        {
            // Arrange
            var mealId = Guid.NewGuid();

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetMealDetailsQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("GetMealDetailsQuery was not sent");

            // Act
            await _mealsController.GetMealDetails(mealId);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<GetMealDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockedMediator.Verify(x => x.Send(It.Is<GetMealDetailsQuery>(x => x.MealId == mealId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void GivenNotNullMediatorResult_WhenGetMealDetails_ThenReturnsOK()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetMealDetailsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new MealDTO(new Meal())))
                .Verifiable("GetMealDetailsQuery was not sent");

            // Act 
            var result = await _mealsController.GetMealDetails(Guid.NewGuid());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeActionResult.StatusCode);
        }

        [Fact]
        public async void GivenNullMediatorResult_WhenGetMealDetails_ThenReturnsNotFound()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetMealDetailsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((MealDTO)null))
                .Verifiable("GetMealDetailsQuery was not sent");

            // Act 
            var result = await _mealsController.GetMealDetails(Guid.NewGuid());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCodeActionResult.StatusCode);
        }
    }
}
