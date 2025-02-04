using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
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
    public class AnswerComplaintCommandHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly AnswerComplaintWithIdCommandHandler _commandHandler;

        public AnswerComplaintCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _commandHandler = new AnswerComplaintWithIdCommandHandler(_dbContext);
        }

        [Fact]
        public async Task GivenComplaintIdThatNotExists_WhenHandle_ThenReturnsComplaintExistsAsFalse()
        {
            // Arrange
            var command = new AnswerComplaintWithIdCommand(new AnswerComplaintCommand(), Guid.NewGuid());

            // Act
            var result = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.complaintExists);
            Assert.False(result.complaintAnswered);
        }

        [Fact]
        public async Task GivenComplaintThatIsAlreadyAnswered_WhenHandle_ThenReturnsComplaintAnsweredAsFalse()
        {
            // Arrange
            var complaintInDb = new Complaint
            {
                Id = Guid.NewGuid(),
                Status = ComplaintStatus.Closed
            };
            _dbContext.Complaints.Add(complaintInDb);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _commandHandler.Handle(new AnswerComplaintWithIdCommand(new AnswerComplaintCommand(), complaintInDb.Id),
                CancellationToken.None);

            // Assert
            Assert.True(result.complaintExists);
            Assert.False(result.complaintAnswered);
        }

        [Fact]
        public async Task GivenComplaintThatIsToOpened_WhenHandle_ThenReturnsComplaintExistsAndComplaintAnswererdAsTrue()
        {
            // Arrange
            var complaintInDb = new Complaint
            {
                Id = Guid.NewGuid(),
                Status = ComplaintStatus.Opened
            };
            _dbContext.Complaints.Add(complaintInDb);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _commandHandler.Handle(new AnswerComplaintWithIdCommand(new AnswerComplaintCommand(), complaintInDb.Id),
                CancellationToken.None);

            // Assert
            Assert.True(result.complaintExists);
            Assert.True(result.complaintAnswered);
        }

        [Fact]
        public async Task GivenComplaintThatIsOpen_WhenHandle_ThenChangesComplaintStatusToClosedInDatabase()
        {
            // Arrange
            var complaintInDb = new Complaint
            {
                Id = Guid.NewGuid(),
                Status = ComplaintStatus.Opened,
            };
            _dbContext.Complaints.Add(complaintInDb);
            await _dbContext.SaveChangesAsync();

            var command = new AnswerComplaintWithIdCommand(
                new AnswerComplaintCommand() { Compliant_answer = "Test answer" },
                complaintInDb.Id);

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            var complaint = _dbContext.Complaints.First();
            Assert.Equal(complaint.Answer, command.Compliant_answer);
            Assert.Equal(ComplaintStatus.Closed, complaint.Status);
        }
    }
}
