using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Diets.Commands;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Commands;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.DietsHandlers
{
    public class AddDietCommandHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly AddDietCommandHandler _addDietCommandHandler;

        public AddDietCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _addDietCommandHandler = new AddDietCommandHandler(_dbContext);
        }

        [Fact]
        public async Task GivenDietWithNameThatIsAlreadyAvailable_WhenHandleAddDietCommand_ThenReturnsDietAddedAsFalse()
        {
            // Arrange
            _dbContext.Diets.Add(new Diet
            {
                Title = "Diet",
                IsAvailable = true
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _addDietCommandHandler.Handle(new AddDietCommand {Name = "Diet"}, CancellationToken.None);

            // Assert
            Assert.False(result.dietAdded);
        }

        [Fact]
        public async Task GivenNotExistingMealId_WhenHandleAddDietCommand_ThenReturnsDietAddedAsFalse()
        {
            // Act
            var result = await _addDietCommandHandler.Handle(new AddDietCommand { Name = "Diet", MealIds = new []{Guid.NewGuid()}}, CancellationToken.None);

            // Assert
            Assert.False(result.dietAdded);
        }

        [Fact]
        public async Task GivenCorrectAddDietCommand_WhenHandleAddDietCommand_ThenAddsDietCorrectlyAndReturnTrue()
        {
            // Arrange
            var meal1Id = Guid.NewGuid();

            _dbContext.Meals.Add(new Meal
            {
                Id = meal1Id,
                IsAvailable = true
            });
            await _dbContext.SaveChangesAsync();

            var addDietCommand = new AddDietCommand
            {
                Description = "myDescription",
                MealIds = new[] {meal1Id},
                Name = "myDiet",
                Price = 10
            };

            // Act
            var result = await _addDietCommandHandler.Handle(addDietCommand, CancellationToken.None);

            // Assert
            Assert.True(result.dietAdded);

            var addedDiet = await _dbContext.Diets.FirstOrDefaultAsync();
            Assert.NotNull(addedDiet);
            Assert.Equal(addDietCommand.Description, addedDiet.Description);
            Assert.Equal(addDietCommand.MealIds.Length, addedDiet.Meals.Count);
            Assert.Equal(addDietCommand.MealIds[0], addedDiet.Meals.First().Id);
            Assert.Equal(addDietCommand.Name, addedDiet.Title);
            Assert.Equal(addDietCommand.Price, addedDiet.Price);
        }
    }
}
