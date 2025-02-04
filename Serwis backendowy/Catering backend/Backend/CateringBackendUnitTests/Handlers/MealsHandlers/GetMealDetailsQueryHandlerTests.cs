using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Queries;
using CateringBackend.Utilities.Extensions;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.MealsHandlers
{
    public class GetMealDetailsQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly GetMealDetailsQueryHandler _getMealDetailsQueryHandler;

        public GetMealDetailsQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _getMealDetailsQueryHandler = new GetMealDetailsQueryHandler(_dbContext);
        }

        [Fact]
        public async Task GivenGetMealDetailsQueryWithExistingMealId_WhenHandle_ThenReturnsValidMealDetailsDTO()
        {
            // Arrange
            _dbContext.Meals.Add(Meal.Create("MealName", "ing1, ing2, ing3", "alg1, alg2", 100, true));
            await _dbContext.SaveChangesAsync();
            var mealInDatabase = _dbContext.Meals.First();
            var getMealDetailsQuery = new GetMealDetailsQuery(mealInDatabase.Id);

            // Act
            var result = await _getMealDetailsQueryHandler.Handle(getMealDetailsQuery, CancellationToken.None);

            // Assert
            Assert.Equal(getMealDetailsQuery.MealId, result.MealId);
            Assert.Equal(mealInDatabase.Calories, result.Calories);
            Assert.Equal(mealInDatabase.Name, result.Name);
            Assert.Equal(mealInDatabase.IsVegan, result.Vegan);
            Assert.Equal(mealInDatabase.Allergens.SplitByCommaToArray(), result.AllergenList);
            Assert.Equal(mealInDatabase.Ingredients.SplitByCommaToArray(), result.IngredientList);
        }

        [Fact]
        public async Task GivenGetMealDetailsQueryWithNotExistingMealId_WhenHandle_ThenReturnsNull()
        {
            // Arrange
            var getMealDetailsQuery = new GetMealDetailsQuery(Guid.NewGuid());

            // Act
            var result = await _getMealDetailsQueryHandler.Handle(getMealDetailsQuery, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
