using System.Collections.Generic;
using CateringBackend.Domain.Entities;
using CateringBackend.Users.Client.Commands;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public static class EditClientWithIdCommandHandlerTestsData
    {
        public static IEnumerable<object[]> GetEditClientCommandAndClient()
        {
            var validClientInDatabase = new Client
            {
                Address = new Address
                {
                    ApartmentNumber = "123",
                    Street = "ClientStreet",
                    City = "ClientCity",
                    PostCode = "ClientPostCode",
                    BuildingNumber = "ClientBuildingNumber"
                },
                Email = "ClientEmail@gmail.com",
                FirstName = "ClientFirstName",
                LastName = "ClientLastName",
                Password = "asdfasdf",
                PhoneNumber = "123456789",
            };

            yield return new object[]
            {
                validClientInDatabase,
                new EditClientCommand
                {
                    Address = new RegisterClientAddress
                    {
                        City = "EditedCity",
                        ApartmentNumber = "EditedApartmentNumber",
                        Street = "EditedStreet",
                        BuildingNumber = "EditedBuildingNumber",
                        PostCode = "EditedPostCode",
                    },
                    Email = "EditedEmail@gmail.com",
                    LastName = "EditedLastName",
                    Name = "EditedName",
                    Password = "EditedPassword",
                    PhoneNumber = "+48987654321",
                }
            };
        }
    }
}
