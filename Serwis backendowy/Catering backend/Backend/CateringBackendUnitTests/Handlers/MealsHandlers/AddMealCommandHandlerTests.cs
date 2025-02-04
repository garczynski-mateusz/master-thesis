using System;
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
    public class AddMealCommandHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly AddMealCommandHandler _addMealCommandHandler;
        public AddMealCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _addMealCommandHandler = new AddMealCommandHandler(_dbContext);
        }

        [Fact]
        public async Task GivenMealWithNameThatIsUsedByAvailableMeal_WhenHandleAddMealCommand_ThenReturnsFalse()
        {
            // Arrange
            const string testMealName = "testMeal";

            _dbContext.Meals.Add(new Meal
            {
                Name = testMealName,
                IsAvailable = true,
            });
            await _dbContext.SaveChangesAsync();

            var addMealCommandHandler = new AddMealCommandHandler(_dbContext);
            var addMealCommand = new AddMealCommand
            {
                Name = testMealName
            };

            // Act
            var result = await addMealCommandHandler.Handle(addMealCommand, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(AddMealCommandHandlerTestsData.GetValidAddMealCommands),
            MemberType = typeof(AddMealCommandHandlerTestsData))]
        public async Task GivenAddMealCommand_WhenHandle_ThenAddsCorrectMealToDatabase(
            AddMealCommand addMealCommand)
        {
            // Act
            await _addMealCommandHandler.Handle(addMealCommand, CancellationToken.None);

            // Assert
            AssertMealAddedToDatabase(_dbContext, addMealCommand);
        }

        [Theory]
        [MemberData(nameof(AddMealCommandHandlerTestsData.GetValidAddMealCommands),
            MemberType = typeof(AddMealCommandHandlerTestsData))]
        public async Task GivenAddMealCommand_WhenHandle_ThenReturnsTrue(
            AddMealCommand addMealCommand)
        {
            // Act
            var result = await _addMealCommandHandler.Handle(addMealCommand, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        private static void AssertMealAddedToDatabase(CateringDbContext dbContext,
            AddMealCommand addMealCommand)
        {
            Assert.Equal(1, dbContext.Meals.Count());
            var addedMeal = dbContext.Meals.First();
            Assert.Equal(addMealCommand.Name, addedMeal.Name);
            Assert.Equal(addMealCommand.Calories, addedMeal.Calories);
            Assert.Equal(addMealCommand.Vegan, addedMeal.IsVegan);

            if (addMealCommand.IngredientList != null)
                Assert.Equal(string.Join(',', addMealCommand.IngredientList), addedMeal.Ingredients);
            else
                Assert.Equal(string.Empty, addedMeal.Ingredients);

            if (addMealCommand.AllergenList != null)
                Assert.Equal(string.Join(',', addMealCommand.AllergenList), addedMeal.Allergens);
            else
                Assert.Equal(string.Empty, addedMeal.Allergens);
        }
    }
}