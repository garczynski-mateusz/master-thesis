using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Entities.Enums;
using CateringBackend.Users.Client.Commands;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public class AddComplaintCommandHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly AddComplaintCommandWithClientIdHandler _commandHandler;

        public AddComplaintCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _commandHandler = new AddComplaintCommandWithClientIdHandler(_dbContext);
        }

        [Fact]
        public async Task GivenOrderIdThatNotExists_WhenHandle_ThenReturnsOrderExistsAsFalse()
        {
            // Arrange
            var command = new AddComplaintCommandWithClientId(new AddComplaintCommand(),
                Guid.NewGuid(), Guid.NewGuid());

            // Act
            var result = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.orderExists);
            Assert.False(result.complaintAdded);
        }

        [Fact]
        public async Task GivenCorrectAddComplaintCommand_WhenHandle_ThenAddsComplaintInTheDatabase()
        {
            // Arrange
            var orderInDb = new Order
            {
                Id = Guid.NewGuid(),
                ClientId = Guid.NewGuid()
            };
            _dbContext.Orders.Add(orderInDb);
            await _dbContext.SaveChangesAsync();

            var command = new AddComplaintCommandWithClientId(
                new AddComplaintCommand
                {
                    Complain_description = "test"
                }, orderInDb.ClientId, orderInDb.Id);

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            var complaint = _dbContext.Complaints.First();
            Assert.Equal(complaint.Description, command.Complain_description);
            Assert.Equal(ComplaintStatus.Opened, complaint.Status);
        }

    }
}
