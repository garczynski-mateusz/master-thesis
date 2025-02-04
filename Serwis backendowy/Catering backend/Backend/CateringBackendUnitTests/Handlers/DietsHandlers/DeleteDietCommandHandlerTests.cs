using CateringBackend.Diets.Commands;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Handlers.DietsHandlers
{
    public class DeleteDietCommandHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly DeleteDietCommandHandler _deleteDietCommandHandler;

        public DeleteDietCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _deleteDietCommandHandler = new DeleteDietCommandHandler(_dbContext);
        }

        [Fact]
        public async Task GivenDietIdThatNotExist_WhenHandleDeleteDietCommand_ThenReturnsDietExistsFalse()
        {
            // Arrange
            var deleteDietCommand = new DeleteDietCommand(Guid.NewGuid());

            // Act
            var (dietExists, errorMessage) = await _deleteDietCommandHandler.Handle(deleteDietCommand, CancellationToken.None);

            // Assert
            Assert.False(dietExists);        }

        [Fact]
        public async Task GivenDietIdThatIsUnavailable_WhenHandleDeleteDietCommand_ThenReturnsDietExistsFalse()
        {
            // Arrange
            var dietToAddToDatabase = new Diet
            {
                Id = Guid.NewGuid(),
                IsAvailable = false
            };

            _dbContext.Diets.Add(dietToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            var deleteDietCommand = new DeleteDietCommand(dietToAddToDatabase.Id);

            // Act
            var (dietExists, errorMessage) = await _deleteDietCommandHandler.Handle(deleteDietCommand, CancellationToken.None);

            // Assert
            Assert.False(dietExists);
        }
    
        [Theory]
        [MemberData(nameof(DeleteDietCommandHandlerTestsData.GetDeleteDietCommandAndDiet),
            MemberType = typeof(DeleteDietCommandHandlerTestsData))]
        public async Task GivenCorrectDeleteDietCommand_WhenHandle_ThenChangesDietAvailabilityInDatabase(
            DeleteDietCommand deleteDietCommand, Diet dietToAddToDatabase)
        {
            // Arrange
            _dbContext.Diets.Add(dietToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            await _deleteDietCommandHandler.Handle(deleteDietCommand, CancellationToken.None);

            // Assert
            var dietFromDatabase = _dbContext.Diets.First();
            Assert.False(dietFromDatabase.IsAvailable);
        }

        [Theory]
        [MemberData(nameof(DeleteDietCommandHandlerTestsData.GetDeleteDietCommandAndDiet),
            MemberType = typeof(DeleteDietCommandHandlerTestsData))]
        public async Task GivenCorrectDeleteDietCommand_WhenHandle_ThenReturnsDietExistsAsTrue(
            DeleteDietCommand deleteDietCommand, Diet dietToAddToDatabase)
        {
            // Arrange
            _dbContext.Diets.Add(dietToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var (dietExists, errorMessage) = await _deleteDietCommandHandler.Handle(deleteDietCommand, CancellationToken.None);

            // Assert
            Assert.True(dietExists);
        }
    }
}
