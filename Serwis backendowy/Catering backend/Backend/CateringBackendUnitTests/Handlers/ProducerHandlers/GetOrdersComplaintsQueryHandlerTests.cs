using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Entities.Enums;
using CateringBackend.Users.Producer.Commands;
using CateringBackend.Users.Producer.Queries;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Handlers.ProducerHandlers
{
    public class GetOrdersComplaintsQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly GetOrdersComplaintsQueryHandler _queryHandler;

        public GetOrdersComplaintsQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _queryHandler = new GetOrdersComplaintsQueryHandler(_dbContext);
        }
        
        [Fact]
        public async Task GivenGetOrdersComplaintsQuery_WhenHandle_ThenReturnAllTheComplaintsFromTheDatabase()
        {
            // Arrange
            var complaintsToAdd = new List<Complaint>
            {
                new() { Description = "Test Description", Answer = null },
                new() { Description = "Test Description", Answer = "Test Answer" },
                new() { Description = "Test Description", Status = ComplaintStatus.Closed },
                new() { Description = "Test Description", Status = ComplaintStatus.Opened },
            };
            _dbContext.Complaints.AddRange(complaintsToAdd);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _queryHandler.Handle(new GetOrdersComplaintsQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(complaintsToAdd.Count(), result.Count());
        }
    }
}
