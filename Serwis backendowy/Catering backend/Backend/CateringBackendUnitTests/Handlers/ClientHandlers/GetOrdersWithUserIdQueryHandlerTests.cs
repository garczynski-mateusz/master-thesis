using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Users.Client.Queries;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public class GetOrdersWithUserIdQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly GetOrdersQueryWithUserIdHandler _queryHandler;

        public GetOrdersWithUserIdQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _queryHandler = new GetOrdersQueryWithUserIdHandler(_dbContext);
        }

        [Theory]
        [MemberData(nameof(GetOrdersWithUserIdQueryHandlerTestsData.GetEmptyOrdresWithEmptyGetOrdersQuery),
            MemberType = typeof(GetOrdersWithUserIdQueryHandlerTestsData))]
        public async void GivenEmptyGetOrdersQuery_WhenHandle_ThenReturnsAllTheClientsOrders(
            List<Order> ordersToAddToDatabase, GetOrdersQueryWithUserId query, Client clientToAddToDatabase)
        {
            // Arrange
            _dbContext.Orders.AddRange(ordersToAddToDatabase);
            _dbContext.Clients.Add(clientToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var response = await _queryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(response.Count(), ordersToAddToDatabase.Count());
        }

        [Theory]
        [MemberData(nameof(GetOrdersWithUserIdQueryHandlerTestsData.FilterByStartDateData),
            MemberType = typeof(GetOrdersWithUserIdQueryHandlerTestsData))]
        public async void GivenGetOrderQueryWithStartDate_WhenHandle_ThenReturnOnlyOrdersWithGivenStartDate(
            GetOrdersQueryWithUserId query, List<Order> ordersInDatabase, List<Order> expectedResult, Client clientToAddToDatabase)
            => await ExecuteFilterTestCase(query, ordersInDatabase, expectedResult, clientToAddToDatabase);

        [Theory]
        [MemberData(nameof(GetOrdersWithUserIdQueryHandlerTestsData.FilterByStartDateData),
            MemberType = typeof(GetOrdersWithUserIdQueryHandlerTestsData))]
        public async void GivenGetOrderQueryWithSortByPrice_WhenHandle_ThenReturnOrdersSortedByPrice(
            GetOrdersQueryWithUserId query, List<Order> ordersInDatabase, List<Order> expectedResult, Client clientToAddToDatabase)
            => await ExecuteSortTestCase(query, ordersInDatabase, expectedResult, clientToAddToDatabase);

        private async Task ExecuteFilterTestCase(GetOrdersQueryWithUserId query, IEnumerable<Order> ordersInDatabase,
            IEnumerable<Order> expectedResult, Client clientToAddToDatabase)
        {
            // Arrange
            await _dbContext.Orders.AddRangeAsync(ordersInDatabase);
            _dbContext.Clients.Add(clientToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _queryHandler.Handle(query, CancellationToken.None);

            // Assert     
            var resultIdsSet = new HashSet<Guid>(result.Select(o => o.Id));
            var expectedIdsSet = new HashSet<Guid>(expectedResult.Select(o => o.Id));
            Assert.Equal(expectedIdsSet, resultIdsSet);
        }

        private async Task ExecuteSortTestCase(GetOrdersQueryWithUserId query, IEnumerable<Order> ordersInDatabase,
            IEnumerable<Order> expectedResult, Client clientToAddToDatabase)
        { 
            // Arrange
            await _dbContext.Orders.AddRangeAsync(ordersInDatabase);
            _dbContext.Clients.Add(clientToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _queryHandler.Handle(query, CancellationToken.None);

            // Assert     
            var resultIdsList = new List<Guid>(result.Select(o => o.Id));
            var expectedIdsList = new List<Guid>(expectedResult.Select(o => o.Id));
            Assert.Equal(expectedIdsList, expectedIdsList);
        }
    }
}
