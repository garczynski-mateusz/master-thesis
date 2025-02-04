using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities.Enums;
using CateringBackend.Users.Producer.Commands;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Handlers.ProducerHandlers
{
    public class CompleteOrderCommandHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly CompleteOrderCommandHandler _completeOrderCommandHandler;

        public CompleteOrderCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _completeOrderCommandHandler = new CompleteOrderCommandHandler(_dbContext);
        }

        [Fact]
        public async Task GivenOrderIdThatNotExists_WhenHandle_ThenReturnsOrderExistsAsFalse()
        {
            // Act
            var result = await _completeOrderCommandHandler.Handle(new CompleteOrderCommand(Guid.NewGuid()), CancellationToken.None);

            // Assert
            Assert.False(result.orderExists);
            Assert.False(result.orderCompleted);
        }

        [Fact]
        public async Task GivenOrderThatIsNotToRealized_WhenHandle_ThenReturnsOrderExistsAsFalse()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _dbContext.Orders.Add(new() { Id = orderId, Status = OrderStatus.WaitingForPayment });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _completeOrderCommandHandler.Handle(new CompleteOrderCommand(orderId), CancellationToken.None);

            // Assert
            Assert.False(result.orderExists);
            Assert.False(result.orderCompleted);
        }

        [Fact]
        public async Task GivenOrderThatIsToRealized_WhenHandle_ThenReturnsOrderExistsAndOrderCompletedAsTrue()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _dbContext.Orders.Add(new() { Id = orderId, Status = OrderStatus.ToRealized });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _completeOrderCommandHandler.Handle(new CompleteOrderCommand(orderId), CancellationToken.None);

            // Assert
            Assert.True(result.orderExists);
            Assert.True(result.orderCompleted);
        }

        [Fact]
        public async Task GivenOrderThatIsToRealized_WhenHandle_ThenChangesOrderStatusToPreparedInDatabase()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _dbContext.Orders.Add(new() { Id = orderId, Status = OrderStatus.ToRealized });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _completeOrderCommandHandler.Handle(new CompleteOrderCommand(orderId), CancellationToken.None);

            // Assert
            var order = _dbContext.Orders.First();
            Assert.Equal(OrderStatus.Prepared, order.Status);
        }
    }
}
