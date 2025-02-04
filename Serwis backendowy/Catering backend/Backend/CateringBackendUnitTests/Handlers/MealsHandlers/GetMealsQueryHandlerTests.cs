using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Queries;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.MealsHandlers
{
    public class GetMealsQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly GetMealsQueryHandler _getMealsQueryHandler;

        public GetMealsQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _getMealsQueryHandler = new GetMealsQueryHandler(_dbContext);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(3, 2)]
        [InlineData(5, 2)]
        [InlineData(2, 0)]
        [InlineData(0, 0)]
        public async void GivenEmptyGetMealsQuery_WhenHandle_ThenReturnsOnlyAvailableMeals(int numberOfMealsToAdd, int numberOfMealsUnavailable)
        {
            // Arrange
            var mealsToSeedDatabase = new List<Meal>();
            for(var i = 0; i < numberOfMealsToAdd; i++)
            {
                mealsToSeedDatabase.Add(Meal.Create(i.ToString(), i.ToString(), i.ToString(), i,true));
            }
            for (var i = 0; i < numberOfMealsUnavailable; i++)
            {
                mealsToSeedDatabase[i].MakeUnavailable(); 
            }
            _dbContext.Meals.AddRange(mealsToSeedDatabase);
            await _dbContext.SaveChangesAsync();
            var getMealsQuery = new GetMealsQuery();

            // Act
            var response = await _getMealsQueryHandler.Handle(getMealsQuery, CancellationToken.None);

            // Assert
            Assert.Equal(numberOfMealsToAdd - numberOfMealsUnavailable, response.Count());

            var databaseAvailableMealsIds = new HashSet<Guid>(_dbContext.Meals.Where(m => m.IsAvailable).Select(m => m.Id));
            var responseMealsIds = new HashSet<Guid>(response.Select(m => m.MealId));
            Assert.Equal(databaseAvailableMealsIds, responseMealsIds);
        }

        [Theory]
        [MemberData(nameof(GetMealsQueryHandlerTestsData.FilterByNameData),
            MemberType = typeof(GetMealsQueryHandlerTestsData))]
        public async void GivenGetMealsQueryWithName_WhenHandle_ThenReturnsOnlyMealsWithGivenName(
            GetMealsQuery query, List<Meal> mealsInDatabase, List<Meal> expectedResult)
            => await ExecuteFilterTestCase(query, mealsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetMealsQueryHandlerTestsData.FilterByNameWithData),
            MemberType = typeof(GetMealsQueryHandlerTestsData))]
        public async void GivenGetMealsQueryWithNameWith_WhenHandle_ThenReturnsOnlyMealsWhichContainsThisPhrase(
            GetMealsQuery query, List<Meal> mealsInDatabase, List<Meal> expectedResult) 
            => await ExecuteFilterTestCase(query, mealsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetMealsQueryHandlerTestsData.FilterByVeganData),
            MemberType = typeof(GetMealsQueryHandlerTestsData))]
        public async void GivenGetMealsQueryWithVegan_WhenHandle_ThenReturnsOnlyMealsWhichMatchVeganValue(
            GetMealsQuery query, List<Meal> mealsInDatabase, List<Meal> expectedResult)
            => await ExecuteFilterTestCase(query, mealsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetMealsQueryHandlerTestsData.FilterByCaloriesData),
            MemberType = typeof(GetMealsQueryHandlerTestsData))]
        public async void GivenGetMealsQueryWithCalories_WhenHandle_ThenReturnsOnlyMealsWhichMatchCaloriesValue(
            GetMealsQuery query, List<Meal> mealsInDatabase, List<Meal> expectedResult)
            => await ExecuteFilterTestCase(query, mealsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetMealsQueryHandlerTestsData.FilterByCaloriesLessThanData),
            MemberType = typeof(GetMealsQueryHandlerTestsData))]
        public async void GivenGetMealsQueryWithCaloriesLt_WhenHandle_ThenReturnsProperMeals(
            GetMealsQuery query, List<Meal> mealsInDatabase, List<Meal> expectedResult)
            => await ExecuteFilterTestCase(query, mealsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetMealsQueryHandlerTestsData.FilterByCaloriesHigherThanData),
            MemberType = typeof(GetMealsQueryHandlerTestsData))]
        public async void GivenGetMealsQueryWithCaloriesHt_WhenHandle_ThenReturnsProperMeals(
            GetMealsQuery query, List<Meal> mealsInDatabase, List<Meal> expectedResult)
            => await ExecuteFilterTestCase(query, mealsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetMealsQueryHandlerTestsData.SortByTitleData),
            MemberType = typeof(GetMealsQueryHandlerTestsData))]
        public async void GivenGetMealsQueryWithSortByTitleData_WhenHandle_ThenReturnsMealsSortedByName(
            GetMealsQuery query, List<Meal> mealsInDatabase, List<Meal> expectedResult)
            => await ExecuteSortTestCase(query, mealsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetMealsQueryHandlerTestsData.SortByCaloriesData),
            MemberType = typeof(GetMealsQueryHandlerTestsData))]
        public async void GivenGetMealsQueryWithSortByCalories_WhenHandle_ThenReturnsMealsSortedByCalories(
            GetMealsQuery query, List<Meal> mealsInDatabase, List<Meal> expectedResult)
            => await ExecuteSortTestCase(query, mealsInDatabase, expectedResult);

        private async Task ExecuteFilterTestCase(GetMealsQuery query, IEnumerable<Meal> mealsInDatabase, 
            IEnumerable<Meal> expectedResult)
        {
            // Arrange
            await _dbContext.Meals.AddRangeAsync(mealsInDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _getMealsQueryHandler.Handle(query, CancellationToken.None);

            // Assert     
            var resultIdsSet = new HashSet<Guid>(result.Select(m => m.MealId));
            var expectedIdsSet = new HashSet<Guid>(expectedResult.Select(m => m.Id));
            Assert.Equal(expectedIdsSet, resultIdsSet);
        }

        private async Task ExecuteSortTestCase(GetMealsQuery query, IEnumerable<Meal> mealsInDatabase,
            IEnumerable<Meal> expectedResult)
        {
            // Arrange
            await _dbContext.Meals.AddRangeAsync(mealsInDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _getMealsQueryHandler.Handle(query, CancellationToken.None);

            // Assert     
            var resultIdsList = new List<Guid>(result.Select(m => m.MealId));
            var expectedIdsList = new List<Guid>(expectedResult.Select(m => m.Id));
            Assert.Equal(expectedIdsList, expectedIdsList);
        }
    }
}
