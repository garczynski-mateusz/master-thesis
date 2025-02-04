using System;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Utilities;
using CateringBackend.Users.Producer.Queries;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.ProducerHandlers
{
    public class LoginProducerQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly LoginProducerQueryHandler _handler;

        public LoginProducerQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _handler = new LoginProducerQueryHandler(_dbContext);
        }

        [Theory]
        [InlineData("producer@gmail.com", "producer123")]
        public async void GivenCorrectCredentials_WhenHandleLoginProducerQuery_ThenReturnsNotNull(string email, string password)
        {
            // Arrange
            _dbContext.Producers.Add(new Producer
            {
                Email = email,
                Password = PasswordManager.Encrypt(password),
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var res = await _handler.Handle(new LoginProducerQuery
                { Email = email, Password = password }, default);

            // Assert
            Assert.NotNull(res);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("producer@gmail.com", null)]
        [InlineData(null, "producer123")]
        [InlineData("", "")]
        [InlineData("producer@gmail.com", "")]
        [InlineData("", "producer123")]
        [InlineData("    ", " ")]
        [InlineData("producer@gmail.com", "  ")]
        [InlineData("    ", "producer123")]
        [InlineData("notproducer@gmail.com", "notproducer123")]
        [InlineData("notproducer@gmail.com", "producer123")]
        [InlineData("producer@gmail.com", "notproducer123")]
        public async void GivenIncorrectCredentials_WhenHandleLoginProducerQuery_ThenReturnsNull(string email, string password)
        {
            // Arrange 
            _dbContext.Producers.Add(new Producer
            {
                Email = "producer@gmail.com",
                Password = PasswordManager.Encrypt("producer123"),
            });
            await _dbContext.SaveChangesAsync();
            var queryHandler = new LoginProducerQueryHandler(_dbContext);

            // Act
            var res = await _handler.Handle(new LoginProducerQuery
            { Email = email, Password = password }, default);

            // Assert
            Assert.Null(res);
        }
    }
}