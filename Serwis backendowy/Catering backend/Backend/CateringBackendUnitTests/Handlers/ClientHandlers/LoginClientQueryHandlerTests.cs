using System;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Utilities;
using CateringBackend.Users.Client.Queries;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public class LoginClientQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly LoginClientQueryHandler _handler;

        public LoginClientQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _handler = new LoginClientQueryHandler(_dbContext);
        }

        [Theory]
        [InlineData("client@gmail.com", "client123")]
        public async void GivenCorrectCredentials_WhenHandleLoginClientQuery_ThenReturnsNotNull(string email, string password)
        {
            // Arrange
            _dbContext.Clients.Add(new Client
            {
                Email = email,
                Password = PasswordManager.Encrypt(password),
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var res = _handler.Handle(new LoginClientQuery
            { Email = email, Password = password }, default);

            // Assert
            Assert.NotNull(res.Result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("client@gmail.com", null)]
        [InlineData(null, "client123")]
        [InlineData("", "")]
        [InlineData("client@gmail.com", "")]
        [InlineData("", "client123")]
        [InlineData("    ", " ")]
        [InlineData("client@gmail.com", "  ")]
        [InlineData("    ", "client123")]
        [InlineData("notclient@gmail.com", "notclient123")]
        [InlineData("notclient@gmail.com", "client123")]
        [InlineData("client@gmail.com", "notclient123")]
        public async void GivenIncorrectCredentials_WhenHandleLoginClientQuery_ThenReturnsNull(string email, string password)
        {
            // Arrange 
            _dbContext.Clients.Add(new Client
            {
                Email = "client@gmail.com",
                Password = PasswordManager.Encrypt("client123"),
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var res = _handler.Handle(new LoginClientQuery
            { Email = email, Password = password }, default);

            // Assert
            Assert.Null(res.Result);
        }
    }
}
