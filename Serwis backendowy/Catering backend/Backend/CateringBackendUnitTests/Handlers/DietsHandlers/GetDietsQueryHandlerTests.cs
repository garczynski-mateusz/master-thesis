using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Diets.Queries;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.DietsHandlers
{
    public class GetDietsQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly GetDietsQueryHandler _getDietsQueryHandler;

        public GetDietsQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _getDietsQueryHandler = new GetDietsQueryHandler(_dbContext);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(3, 2)]
        [InlineData(5, 2)]
        [InlineData(2, 0)]
        [InlineData(0, 0)]
        public async void GivenEmptyGetDietsQuery_WhenHandle_ThenReturnsOnlyAvailableDiets(int numberOfDietsToAdd, int numberOfDietsUnavailable)
        {
            // Arrange
            var dietsToSeedDatabase = new List<Diet>();
            for(var i = 0; i < numberOfDietsToAdd; i++)
            {
                dietsToSeedDatabase.Add(Diet.Create(i.ToString(), i.ToString(), i * 10));
            }
            for (var i = 0; i < numberOfDietsUnavailable; i++)
            {
                dietsToSeedDatabase[i].MakeUnavailable(); 
            }
            _dbContext.Diets.AddRange(dietsToSeedDatabase);
            await _dbContext.SaveChangesAsync();
            var GetDietsQuery = new GetDietsQuery();

            // Act
            var response = await _getDietsQueryHandler.Handle(GetDietsQuery, CancellationToken.None);

            // Assert
            Assert.Equal(numberOfDietsToAdd - numberOfDietsUnavailable, response.Count());

            var databaseAvailableDietsIds = new HashSet<Guid>(_dbContext.Diets.Where(d => d.IsAvailable).Select(d => d.Id));
            var responseDietIds = new HashSet<Guid>(response.Select(d => d.Id));
            Assert.Equal(databaseAvailableDietsIds, responseDietIds);
        }

        [Theory]
        [MemberData(nameof(GetDietsQueryHandlerTestsData.FilterByNameData),
            MemberType = typeof(GetDietsQueryHandlerTestsData))]
        public async void GivenGetDietsQueryWithName_WhenHandle_ThenReturnsOnlyDietsWithGivenName(
            GetDietsQuery query, List<Diet> dietsInDatabase, List<Diet> expectedResult)
            => await ExecuteFilterTestCase(query, dietsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetDietsQueryHandlerTestsData.FilterByNameWithData),
            MemberType = typeof(GetDietsQueryHandlerTestsData))]
        public async void GivenGetDietsQueryWithNameWith_WhenHandle_ThenReturnsOnlyDietsWhichContainsThisPhrase(
            GetDietsQuery query, List<Diet> dietsInDatabase, List<Diet> expectedResult)
            => await ExecuteFilterTestCase(query, dietsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetDietsQueryHandlerTestsData.FilterByVeganData),
            MemberType = typeof(GetDietsQueryHandlerTestsData))]
        public async void GivenGetDietsQueryWithVegan_WhenHandle_ThenReturnsOnlyDietsWhichMatchVeganValue(
            GetDietsQuery query, List<Diet> dietsInDatabase, List<Diet> expectedResult)
            => await ExecuteFilterTestCase(query, dietsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetDietsQueryHandlerTestsData.FilterByCaloriesData),
            MemberType = typeof(GetDietsQueryHandlerTestsData))]
        public async void GivenGetDietsQueryWithCalories_WhenHandle_ThenReturnsOnlyDietsWhichMatchQueryCaloriesParameter(
            GetDietsQuery query, List<Diet> dietsInDatabase, List<Diet> expectedResult)
            => await ExecuteFilterTestCase(query, dietsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetDietsQueryHandlerTestsData.FilterByPriceData),
            MemberType = typeof(GetDietsQueryHandlerTestsData))]
        public async void GivenGetDietsQueryWithPrice_WhenHandle_ThenReturnsOnlyDietsWhichMatchQueryPriceParameter(
            GetDietsQuery query, List<Diet> dietsInDatabase, List<Diet> expectedResult)
            => await ExecuteFilterTestCase(query, dietsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetDietsQueryHandlerTestsData.SortByTitleData),
            MemberType = typeof(GetDietsQueryHandlerTestsData))]
        public async void GivenGetDietsQueryWithSortByTitleData_WhenHandle_ThenReturnsDietsSortedByName(
            GetDietsQuery query, List<Diet> dietsInDatabase, List<Diet> expectedResult)
            => await ExecuteSortTestCase(query, dietsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetDietsQueryHandlerTestsData.SortByCaloriesData),
            MemberType = typeof(GetDietsQueryHandlerTestsData))]
        public async void GivenGetDietsQueryWithSortByCalories_WhenHandle_ThenReturnsDietsSortedByCalories(
            GetDietsQuery query, List<Diet> dietsInDatabase, List<Diet> expectedResult)
            => await ExecuteSortTestCase(query, dietsInDatabase, expectedResult);

        [Theory]
        [MemberData(nameof(GetDietsQueryHandlerTestsData.SortByPriceData),
            MemberType = typeof(GetDietsQueryHandlerTestsData))]
        public async void GivenGetDietsQueryWithSortByPrice_WhenHandle_ThenReturnsDietsSortedByPrice(
            GetDietsQuery query, List<Diet> dietsInDatabase, List<Diet> expectedResult)
            => await ExecuteSortTestCase(query, dietsInDatabase, expectedResult);


        private async Task ExecuteFilterTestCase(GetDietsQuery query, IEnumerable<Diet> dietsInDatabase,
            IEnumerable<Diet> expectedResult)
        {
            // Arrange
            await _dbContext.Diets.AddRangeAsync(dietsInDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _getDietsQueryHandler.Handle(query, CancellationToken.None);

            // Assert     
            var resultIdsSet = new HashSet<(Guid, string)>(result.Select(m => (m.Id, m.Name)));
            var expectedIdsSet = new HashSet<(Guid, string)>(expectedResult.Select(m => (m.Id, m.Title)));
            Assert.Equal(expectedIdsSet, resultIdsSet);
        }

        private async Task ExecuteSortTestCase(GetDietsQuery query, IEnumerable<Diet> dietsInDatabase,
            IEnumerable<Diet> expectedResult)
        {
            // Arrange
            await _dbContext.Diets.AddRangeAsync(dietsInDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _getDietsQueryHandler.Handle(query, CancellationToken.None);

            // Assert     
            var resultIdsList = new List<(Guid, string)>(result.Select(m => (m.Id, m.Name)));
            var expectedIdsList = new List<(Guid, string)>(expectedResult.Select(m => (m.Id, m.Title)));
            Assert.Equal(expectedIdsList, expectedIdsList);
        }
    }
}
