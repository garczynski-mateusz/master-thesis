using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Entities.Enums;
using CateringBackend.Users.Client.Commands;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public class AddOrderCommandHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly AddOrderCommandHandler _addOrderCommandHandler;

        public AddOrderCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _addOrderCommandHandler = new AddOrderCommandHandler(_dbContext);
        }

        [Theory]
        [MemberData(nameof(AddOrderCommandHandlerTestsData.GetAddOrderCommandsWithInvalidDates),
            MemberType = typeof(AddOrderCommandHandlerTestsData))]
        public async Task GivenAddOrderCommandWithInvalidDates_WhenHandle_ThenReturnsNull(
            AddOrderCommandWithClientId addOrderCommandWithClientId)
        {
            // Act 
            var result = await _addOrderCommandHandler.Handle(addOrderCommandWithClientId, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(AddOrderCommandHandlerTestsData.GetAddOrderCommandWithNonExistentDietIdsWithDietToAdd),
            MemberType = typeof(AddOrderCommandHandlerTestsData))]
        public async Task GivenAddOrderCommandContainingNonExistentDiet_WhenHandle_ThenReturnsNull(
            AddOrderCommandWithClientId addOrderCommandWithClientId, Diet dietToAddToDatabase)
        {
            // Arrange
            _dbContext.Diets.Add(dietToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _addOrderCommandHandler.Handle(addOrderCommandWithClientId, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(AddOrderCommandHandlerTestsData.GetAddOrderCommandWithNullAdressWithDietClientAndClientAddress),
            MemberType = typeof(AddOrderCommandHandlerTestsData))]
        public async Task GivenAddOrderCommandWithNullAddress_WhenHandle_ThenAddOrderWithClientAddressToDatabase(
            AddOrderCommandWithClientId command, Diet dietToAddToDatabase, Client clientToAddToDatabase, Address clientAddress)
        {
            // Arrange
            _dbContext.Diets.Add(dietToAddToDatabase);
            _dbContext.Addresses.Add(clientAddress);
            _dbContext.Clients.Add(clientToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            await _addOrderCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            var deliveryAddressId = _dbContext.Orders.First().DeliveryAddressId;
            Assert.Equal(deliveryAddressId, clientAddress.Id);
        }

        [Theory]
        [MemberData(nameof(AddOrderCommandHandlerTestsData.GetAddOrderCommandWithExistingAddressWithDietAndAddressToAdd),
            MemberType = typeof(AddOrderCommandHandlerTestsData))]
        public async Task GivenAddOrderCommandWithAddressThatIsInDatabase_WhenHandle_ThenAddOrderWithTheIdOfExistentAddress(
            AddOrderCommandWithClientId command, Diet dietToAddToDatabase, Address addressToAddToDatabase)
        {
            // Arrange
            _dbContext.Diets.Add(dietToAddToDatabase);
            _dbContext.Addresses.Add(addressToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            await _addOrderCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            var deliveryAddressId = _dbContext.Orders.First().DeliveryAddressId;
            Assert.Equal(deliveryAddressId, addressToAddToDatabase.Id);
        }

        [Theory]
        [MemberData(nameof(AddOrderCommandHandlerTestsData.GetAddOrderCommandWithNonExistingAddressWithDietToAdd),
            MemberType = typeof(AddOrderCommandHandlerTestsData))]
        public async Task GivenAddOrderCommandWithAddressThatIsNotInDatabase_WhenHandle_ThenAddAddressToDatabaseAndAddOrderWithCorrectAddress(
            AddOrderCommandWithClientId command, Diet dietToAddToDatabase)
        {
            // Arrange
            _dbContext.Diets.Add(dietToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            await _addOrderCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1, _dbContext.Addresses.Count());
            var addedAddress = _dbContext.Addresses.First();
            var addedOrder = _dbContext.Orders.First();
            Assert.Equal(addedAddress.Id, addedOrder.DeliveryAddressId);
        }

        [Theory]
        [MemberData(nameof(AddOrderCommandHandlerTestsData.GetAddOrderCommandWithDietToAdd),
            MemberType = typeof(AddOrderCommandHandlerTestsData))]
        public async Task GivenAddOrderCommand_WhenHandle_ThenIdOfAddedOrderIsReturned(
            AddOrderCommandWithClientId command, Diet dietToAddToDatabase)
        {
            // Arrange
            _dbContext.Diets.Add(dietToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _addOrderCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            var addedOrderId = _dbContext.Orders.First().Id;
            Assert.Equal(addedOrderId.ToString(), result);
        }

        [Theory]
        [MemberData(nameof(AddOrderCommandHandlerTestsData.GetAddOrderCommandWithMultipleDietsToAdd),
            MemberType = typeof(AddOrderCommandHandlerTestsData))]
        public async Task GivenAddOrderCommand_WhenHandle_ThenCorrectOrderIsAddedToDatabase(
            AddOrderCommandWithClientId command, List<Diet> dietsToAddToDatabase)
        {
            // Arrange
            _dbContext.Diets.AddRange(dietsToAddToDatabase);
            await _dbContext.SaveChangesAsync();

            // Act
            await _addOrderCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1, _dbContext.Orders.Count());
            Assert.True(CompareAddOrderCommandWithAddedOrder(command, _dbContext.Orders.First()));
        }

        private bool CompareAddOrderCommandWithAddedOrder(AddOrderCommandWithClientId command, Order addedOrder)
        {
            var dietsInOrder = _dbContext.Diets.Where(d => command.DietIDs.Contains(d.Id)).ToList();

            return addedOrder.Status == OrderStatus.WaitingForPayment &&
                addedOrder.ClientId == command.ClientId &&
                addedOrder.CommentForDeliverer == command.DeliveryDetails.CommentForDeliverer &&
                addedOrder.StartDate == command.StartDate &&
                addedOrder.EndDate == command.EndDate &&
                addedOrder.Price == dietsInOrder.Sum(d => d.Price) * (int)(command.EndDate - command.StartDate).TotalDays;
        }
    }
}
