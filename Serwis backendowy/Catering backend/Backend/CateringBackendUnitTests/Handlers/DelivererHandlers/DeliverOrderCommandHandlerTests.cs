using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Entities.Enums;
using CateringBackend.Users.Deliverer.Commands;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Handlers.DelivererHandlers
{
    public class DeliverOrderCommandHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly DeliverOrderCommandHandler _commandHandler;
        public DeliverOrderCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _commandHandler = new DeliverOrderCommandHandler(_dbContext);
        }

        [Fact]
        public async Task GivenOrderIdThatNotExists_WhenHandle_ThenReturnsFalse()
        {
            // Arrange
            var command = new DeliverOrderCommand() { OrderId = Guid.NewGuid() };

            // Act
            var result = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.orderExists);
        }
        
        [Theory]
        [InlineData(OrderStatus.Created, false)]
        [InlineData(OrderStatus.WaitingForPayment, false)]
        [InlineData(OrderStatus.Paid, false)]
        [InlineData(OrderStatus.ToRealized, false)]
        [InlineData(OrderStatus.Delivered, false)]
        [InlineData(OrderStatus.Finished, false)]
        [InlineData(OrderStatus.Canceled, false)]
        [InlineData(OrderStatus.Prepared, true)]
        public async Task GivenOrderWithStatus_WhenHandle_ThenReturnsProperStatusCode(OrderStatus orderStatus, bool expectedResult)
        {
            // Arrange
            var orderInDb = new Order
            {
                Id = Guid.NewGuid(),
                Status = orderStatus,
                Client = null,
                DeliveryAddress = null,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Price = 100
            };

            _dbContext.Orders.Add(orderInDb);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _commandHandler.Handle(new DeliverOrderCommand() { OrderId = orderInDb.Id }, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResult, result.orderDelivered);
        }
    }
}
