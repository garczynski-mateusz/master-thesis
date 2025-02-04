using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Diets.Queries;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Queries;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.DietsHandlers
{
    public class GetDietDetailsQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly GetDietDetailsQueryHandler _getDietDetailsQueryHandler;

        public GetDietDetailsQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _getDietDetailsQueryHandler = new GetDietDetailsQueryHandler(_dbContext);
        }

        [Fact]
        public async Task GivenQueryForExistingDiet_WhenHandleGetDietDetailsQuery_ThenReturnsValidDTO()
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
                Price = 100
            };

            _dbContext.Diets.Add(dietInDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result =
                await _getDietDetailsQueryHandler.Handle(new GetDietDetailsQuery(dietInDatabase.Id),
                    CancellationToken.None);

            // Assert
            Assert.Equal(dietInDatabase.Title, result.Name);
            Assert.Equal(dietInDatabase.Meals.Sum(x => x.Calories), result.Calories);
            Assert.Equal(dietInDatabase.Meals.All(x => x.IsVegan), result.Vegan);
            Assert.Equal(dietInDatabase.Meals.Count, result.Meals.Count());
            Assert.Equal(dietInDatabase.Price, result.Price);
        }

        [Fact]
        public async Task GivenQueryForNotExistingDiet_WhenHandleGetDietDetailsQuery_ThenReturnsNull()
        {
            // Act 
            var result =
                await _getDietDetailsQueryHandler.Handle(new GetDietDetailsQuery(Guid.NewGuid()),
                    CancellationToken.None);

            //Assert
            Assert.Null(result);
        }

    }
}
