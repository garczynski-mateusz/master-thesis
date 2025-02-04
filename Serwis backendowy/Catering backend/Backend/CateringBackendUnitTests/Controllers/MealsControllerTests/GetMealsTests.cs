using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Controllers;
using CateringBackend.Meals;
using CateringBackend.Meals.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.MealsControllerTests
{
    public class GetMealsTests
    {
        private readonly MealsController _mealsController;
        private readonly Mock<IMediator> _mockedMediator;

        public GetMealsTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _mealsController = new MealsController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenGetMealsQuery_WhenGetMeals_ThenItIsSentToMediator()
        {
            // Arrange
            var getMealsQuery = new GetMealsQuery();

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetMealsQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("GetMealsQuery was not sent");

            // Act
            await _mealsController.GetMeals(getMealsQuery);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<GetMealsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void GivenMediatorResult_WhenGetMeals_ThenReturnsOK()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetMealsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<MealSearchResultDTO>>(new List<MealSearchResultDTO>()))
                .Verifiable("GetMealsQuery was not sent");

            // Act 
            var result = await _mealsController.GetMeals(new GetMealsQuery());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeActionResult.StatusCode);
        }
    }
}
