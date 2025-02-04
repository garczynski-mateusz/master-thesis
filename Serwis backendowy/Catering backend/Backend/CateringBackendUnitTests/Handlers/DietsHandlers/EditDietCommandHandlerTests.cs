using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Diets.Commands;
using CateringBackend.Diets.Queries;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using EntityFrameworkCore.Testing.Moq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Handlers.DietsHandlers
{
    public class EditDietCommandHandlerTests
    {
        private readonly EditDietWithDietIdCommandHandler _editDietCommandHandler;
        private readonly CateringDbContext _dbContext;
        private readonly Mock<IMediator> _mockedMediator;

        public EditDietCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _mockedMediator = new Mock<IMediator>();
            _editDietCommandHandler = new EditDietWithDietIdCommandHandler(_mockedMediator.Object, _dbContext);
        }

        [Fact]
        public async Task GivenEditDietCommandForNotExistingDiet_WhenHandle_ThenReturnsDietExistsAndEditedAsFalse()
        {
            // Act
            var result = await _editDietCommandHandler.Handle(new EditDietWithDietIdCommand(new EditDietCommand(), Guid.NewGuid()),
                CancellationToken.None);

            // Assert
            Assert.False(result.dietExists);
            Assert.False(result.dietEdited);
        }

        [Fact]
        public async Task GivenEditDietCommandForExistingDiet_WhenHandle_ThenInvokesMediatorWithAddDiet()
        {
            // Arrange
            var dietInDatabase = new Diet
            {
                Description = "myDescription",
                Id = Guid.NewGuid(),
                IsAvailable = true,
                Meals = new HashSet<Meal>
                {
                    new() {Name = "meal1", Id = Guid.NewGuid(), Calories = 10, IsVegan = true},
                    new() {Name = "meal2", Id = Guid.NewGuid(), Calories = 20, IsVegan = false},
                },
                Title = "myDiet",
                Price = 1
            };

            _dbContext.Diets.Add(dietInDatabase);
            await _dbContext.SaveChangesAsync();

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AddDietCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("AddDietCommand was not sent");

            var editDietCommand = new EditDietCommand
            {
                Description = "AnotherSuperDescription",
                MealIds = new[] {dietInDatabase.Meals.First().Id},
                Name = "AnotherNameForDiet",
                Price = 2
            };

            // Act 
            await _editDietCommandHandler.Handle(new EditDietWithDietIdCommand(editDietCommand, dietInDatabase.Id), CancellationToken.None);

            // Assert
            _mockedMediator.Verify(
                x => x.Send(
                    It.Is<AddDietCommand>(x =>
                        x.MealIds == editDietCommand.MealIds && x.Name == editDietCommand.Name &&
                        x.Description == editDietCommand.Description && x.Price == editDietCommand.Price),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
