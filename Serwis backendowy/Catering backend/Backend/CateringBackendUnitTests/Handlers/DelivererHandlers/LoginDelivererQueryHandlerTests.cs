using System;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Utilities;
using CateringBackend.Users.Deliverer.Queries;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.DelivererHandlers
{
    public class LoginDelivererQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly LoginDelivererQueryHandler _handler;

        public LoginDelivererQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _handler = new LoginDelivererQueryHandler(_dbContext);
        }

        [Theory]
        [InlineData("deliverer@gmail.com", "deliverer123")]
        public async void GivenCorrectCredentials_WhenHandleLoginDelivererQuery_ThenReturnsNotNull(string email, string password)
        {
            // Arrange
            _dbContext.Deliverers.Add(new Deliverer
            {
                Email = email,
                Password = PasswordManager.Encrypt(password),
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var res = await _handler.Handle(new LoginDelivererQuery()
            { Email = email, Password = password }, default);

            // Assert
            Assert.NotNull(res);
        }
        
        [Theory]
        [InlineData(null, null)]                            
        [InlineData("deliverer@gmail.com", null)]
        [InlineData(null, "deliverer123")]
        [InlineData("", "")]
        [InlineData("deliverer@gmail.com", "")]
        [InlineData("", "deliverer123")]
        [InlineData("    ", " ")]
        [InlineData("deliverer@gmail.com", "  ")]
        [InlineData("    ", "deliverer123")]
        [InlineData("notdeliverer@gmail.com", "notdeliverer123")]
        [InlineData("notdeliverer@gmail.com", "deliverer123")]
        [InlineData("deliverer@gmail.com", "notdeliverer123")]
        public async void GivenIncorrectCredentials_WhenHandleLoginDelivererQuery_ThenReturnsNull(string email, string password)
        {
            // Arrange 
            _dbContext.Deliverers.Add(new Deliverer
            {
                Email = "deliverer@gmail.com",
                Password = PasswordManager.Encrypt("deliverer123"),
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var res = await _handler.Handle(new LoginDelivererQuery
            { Email = email, Password = password }, default);

            // Assert
            Assert.Null(res);
        }
    }
}
