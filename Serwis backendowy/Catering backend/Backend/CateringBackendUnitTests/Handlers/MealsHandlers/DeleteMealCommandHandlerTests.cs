using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Commands;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.MealsHandlers
{
    public class DeleteMealCommandHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly DeleteMealCommandHandler _deleteMealCommandHandler;
        public DeleteMealCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _deleteMealCommandHandler = new DeleteMealCommandHandler(_dbContext);
        }

        [Fact]
        public async Task GivenMealIdThatNotExist_WhenHandleDeleteMealCommand_ThenReturnsNull()
        {
            // Arrange
            var deleteMealCommand = new DeleteMealCommand(Guid.NewGuid());

            // Act
            var result = await _deleteMealCommandHandler.Handle(deleteMealCommand, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GivenMealWithIdThatIsContainedByAvailableDiet_WhenHandleDeleteMealCommand_ThenReturnsAvailableMeal()
        {
            // Arrange 
            var mealToAddToDatabase = new Meal
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            _dbContext.Meals.Add(mealToAddToDatabase);
            _dbContext.Diets.Add(new Diet
            {
                IsAvailable = true,
                Meals = new HashSet<Meal>() { mealToAddToDatabase }
            });
            await _dbContext.SaveChangesAsync();

            var deleteMealCommand = new DeleteMealCommand(mealToAddToDatabase.Id);

            // Act
            var result = await _deleteMealCommandHandler.Handle(deleteMealCommand, CancellationToken.None);

            // Assert
            Assert.True(result.IsAvailable);
        }

        [Theory]
        [MemberData(nameof(DeleteMealCommandHandlerTestsData.GetDeleteMealCommandAndMeal),
            MemberType = typeof(DeleteMealCommandHandlerTestsData))]
        public async Task GivenDeleteMealCommand_WhenHandle_ThenChangesMealAvailabilityInDatabase(
            DeleteMealCommand deleteMealCommand, Meal mealToAddToDatabase)
        {
            // Arrange
            _dbContext.Meals.Add(mealToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            await _deleteMealCommandHandler.Handle(deleteMealCommand, CancellationToken.None);

            // Assert
            var mealFromDatabase = _dbContext.Meals.First();
            Assert.False(mealFromDatabase.IsAvailable);
        }

        [Theory]
        [MemberData(nameof(DeleteMealCommandHandlerTestsData.GetDeleteMealCommandAndMeal),
            MemberType = typeof(DeleteMealCommandHandlerTestsData))]
        public async Task GivenDeleteMealCommand_WhenHandle_ThenReturnsUnavailableMeal(
            DeleteMealCommand deleteMealCommand, Meal mealToAddToDatabase)
        {
            // Arrange
            _dbContext.Meals.Add(mealToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _deleteMealCommandHandler.Handle(deleteMealCommand, CancellationToken.None);

            // Assert
            Assert.False(result.IsAvailable);
        }
    }
}