using System;
using System.Threading;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Exceptions;
using CateringBackend.Users.Client.Queries;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public class GetClientDetailsQueryHandlerTests
    {
        private readonly CateringDbContext _dbContext;
        private readonly GetClientDetailsQueryHandler _getClientDetailsQueryHandler;

        public GetClientDetailsQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CateringDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _dbContext = Create.MockedDbContextFor<CateringDbContext>(options);
            _getClientDetailsQueryHandler = new GetClientDetailsQueryHandler(_dbContext);
        }

        [Fact]
        public async void GivenGetClientDetailsQuery_WhenHandleForNotExistingClient_ThenReturnsNull()
        {
            // Arrange 
            var query = new GetClientDetailsQuery(Guid.NewGuid());

            // Act
            var result = await _getClientDetailsQueryHandler.Handle(query, CancellationToken.None);

            // Assert 
            Assert.Null(result);
        }

        [Fact]
        public async void GivenClientGetClientDetailsQuery_WhenHandleForExistingClientWithoutAddress_ThenThrowsMissingAddressForClientException()
        {
            // Arrange
            var addedClient = _dbContext.Clients.Add(new Client()).Entity;
            await _dbContext.SaveChangesAsync(CancellationToken.None);
;           var query = new GetClientDetailsQuery(addedClient.Id);

            // Act & Assert
            await Assert.ThrowsAsync<MissingAddressForClientException>(async () =>
                await _getClientDetailsQueryHandler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [MemberData(nameof(GetClientDetailsQueryHandlerTestsData.GetValidClients),
            MemberType = typeof(GetClientDetailsQueryHandlerTestsData))]
        public async void GivenGetClientDetailsQuery_WhenHandleForExistingUserWithAddress_ThenReturnsClientDetailsWithAllProperData(Client clientToAddToDatabase)
        {
            // Arrange
            _dbContext.Addresses.Add(clientToAddToDatabase.Address);
            var addedClient = _dbContext.Clients.Add(clientToAddToDatabase).Entity;
            await _dbContext.SaveChangesAsync();
            var query = new GetClientDetailsQuery(addedClient.Id);

            // Act 
            var result = await _getClientDetailsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            AssertClientDetailsDto(result, addedClient)
;       }

        private static void AssertClientDetailsDto(ClientDetailsDto clientDetailsDto, Client client)
        {
            Assert.Equal(clientDetailsDto.Email, client.Email);
            Assert.Equal(clientDetailsDto.LastName, client.LastName);
            Assert.Equal(clientDetailsDto.Name, client.FirstName);
            Assert.Equal(clientDetailsDto.PhoneNumber, client.PhoneNumber);
            Assert.Equal(clientDetailsDto.Address.ApartmentNumber, client.Address.ApartmentNumber);
            Assert.Equal(clientDetailsDto.Address.BuildingNumber, client.Address.BuildingNumber);
            Assert.Equal(clientDetailsDto.Address.City, client.Address.City);
            Assert.Equal(clientDetailsDto.Address.PostCode, client.Address.PostCode);
            Assert.Equal(clientDetailsDto.Address.Street, client.Address.Street);
        }
    }
}
