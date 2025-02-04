using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Users.Deliverer.Queries;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Handlers.DelivererHandlers
{
    public class GetDelivererOrdersQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly GetDelivererOrdersQueryHandler _queryHandler;

        public GetDelivererOrdersQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _queryHandler = new GetDelivererOrdersQueryHandler(_dbContext);
        }

        [Fact]
        public async Task GivenGetDelivererOrdersQueryWithInvalidStatused_WhenHandle_ThenReturnNoOrders()
        {
            // Arrange
            var ordersToAdd = new List<Order>
            {
                new() { Status = CateringBackend.Domain.Entities.Enums.OrderStatus.Created },
                new() { Status = CateringBackend.Domain.Entities.Enums.OrderStatus.WaitingForPayment },
                new() { Status = CateringBackend.Domain.Entities.Enums.OrderStatus.Paid },
                new() { Status = CateringBackend.Domain.Entities.Enums.OrderStatus.ToRealized },
                new() { Status = CateringBackend.Domain.Entities.Enums.OrderStatus.Finished },
                new() { Status = CateringBackend.Domain.Entities.Enums.OrderStatus.Delivered },
            };
            _dbContext.Orders.AddRange(ordersToAdd);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _queryHandler.Handle(new GetDelivererOrdersQuery(), CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GivenGetDelivererOrdersQueryWithStatusToDeliver_WhenHandle_ThenReturnAllOrders()
        {
            // Arrange
            var clientToAdd = new Client
            {
                Id = Guid.NewGuid()
            };
            var ordersToAdd = new List<Order>
            {
                new() { Status = CateringBackend.Domain.Entities.Enums.OrderStatus.Prepared, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), ClientId = clientToAdd.Id },
                new() { Status = CateringBackend.Domain.Entities.Enums.OrderStatus.Prepared, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), ClientId = clientToAdd.Id  },
                new() { Status = CateringBackend.Domain.Entities.Enums.OrderStatus.Prepared, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), ClientId = clientToAdd.Id  },
            };
            _dbContext.Clients.Add(clientToAdd);
            _dbContext.Orders.AddRange(ordersToAdd);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _queryHandler.Handle(new GetDelivererOrdersQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(ordersToAdd.Count(), result.Count());
        }
    }
}
