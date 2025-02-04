using System;
using System.Collections.Generic;
using CateringBackend.Domain.Entities;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public static class GetClientDetailsQueryHandlerTestsData
    {
        public static IEnumerable<object[]> GetValidClients()
        {
            var firstClientAddress = new Guid("f07190b2-9cce-4df7-b22f-2e5be14a7066");
            yield return new object[]
            {
                new Client
                {
                    Address = new Address
                    {
                        Id = firstClientAddress,
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
                    AddressId = firstClientAddress
                }
            };

            var secondClientAddress = new Guid("48aef421-6685-4fab-95d3-66dfb09c34c5");
            yield return new object[]
            {
                new Client
                {
                    Address = new Address
                    {
                        Id = secondClientAddress,
                        ApartmentNumber = "321",
                        Street = "ClientStreet2",
                        City = "ClientCity2",
                        PostCode = "ClientPostCode2",
                        BuildingNumber = "ClientBuildingNumber2"
                    },
                    Email = "ClientEmail2@gmail.com",
                    FirstName = "ClientFirstName2",
                    LastName = "ClientLastName2",
                    Password = "asdfasdf2",
                    PhoneNumber = "987654321",
                    AddressId = secondClientAddress
                }
            };
        }
    }
}
