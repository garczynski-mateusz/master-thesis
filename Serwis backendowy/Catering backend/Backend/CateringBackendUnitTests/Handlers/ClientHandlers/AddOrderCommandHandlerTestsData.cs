using CateringBackend.Domain.Entities;
using CateringBackend.Users;
using CateringBackend.Users.Client.Commands;
using System;
using System.Collections.Generic;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public class AddOrderCommandHandlerTestsData
    {
        public static IEnumerable<object[]> GetAddOrderCommandsWithInvalidDates()
        {
            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    StartDate = DateTime.MaxValue,
                    EndDate = DateTime.MinValue
                }
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    StartDate = DateTime.MinValue,
                    EndDate = DateTime.MinValue
                }
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = DateTime.MaxValue
                }
            };
        }

        public static IEnumerable<object[]> GetAddOrderCommandWithNonExistentDietIdsWithDietToAdd()
        {
            var dietToAddToDatabase = new Diet
            {
                Id = Guid.NewGuid(),
                IsAvailable = false
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    DietIDs = new Guid[] {dietToAddToDatabase.Id},
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2)
                },
                dietToAddToDatabase
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    DietIDs = new Guid[] {Guid.NewGuid()},
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2)
                },
                dietToAddToDatabase
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    DietIDs = new Guid[] { },
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2)
                },
                dietToAddToDatabase
            };
        }

        public static IEnumerable<object[]> GetAddOrderCommandWithNullAdressWithDietClientAndClientAddress()
        {
            var clientAddress = new Address
            {
                Id = Guid.NewGuid(),
                Street = "street",
                BuildingNumber = "buildingNumber",
                ApartmentNumber = "apartmentNumber",
                PostCode = "postCode",
                City = "city"
            };

            var clientToAddToDatabase = new Client
            {
                Id = Guid.NewGuid(),
                AddressId = clientAddress.Id,
                Address = clientAddress
            };

            var dietToAddToDatabase = new Diet
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    ClientId = clientToAddToDatabase.Id,
                    DietIDs = new Guid[] { dietToAddToDatabase.Id },
                    DeliveryDetails = new DeliveryDetailsDTO
                    {
                        Address = null
                    },
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2)
                },
                dietToAddToDatabase,
                clientToAddToDatabase,
                clientAddress
            };
        }

        public static IEnumerable<object[]> GetAddOrderCommandWithExistingAddressWithDietAndAddressToAdd()
        {
            var addressToAddToDatabase = new Address
            {
                Id = Guid.NewGuid(),
                Street = "street",
                BuildingNumber = "buildingNumber",
                ApartmentNumber = "apartmentNumber",
                PostCode = "postCode",
                City = "city"
            };

            var dietToAddToDatabase = new Diet
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    ClientId = Guid.NewGuid(),
                    DietIDs = new Guid[] { dietToAddToDatabase.Id },
                    DeliveryDetails = new DeliveryDetailsDTO
                    {
                        Address = new AddressDTO(addressToAddToDatabase)
                    },
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2)
                },
                dietToAddToDatabase,
                addressToAddToDatabase
            };
        }

        public static IEnumerable<object[]> GetAddOrderCommandWithNonExistingAddressWithDietToAdd()
        {
            var dietToAddToDatabase = new Diet
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    ClientId = Guid.NewGuid(),
                    DietIDs = new Guid[] {dietToAddToDatabase.Id },
                    DeliveryDetails = new DeliveryDetailsDTO
                    {
                        Address = new AddressDTO
                        {
                            Street = "street",
                            BuildingNumber = "buildingNumber",
                            ApartmentNumber = "apartmentNumber",
                            PostCode = "postCode",
                            City = "city"
                        }
                    },
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2)
                },
                dietToAddToDatabase
            };
        }
    
        public static IEnumerable<object[]> GetAddOrderCommandWithDietToAdd()
        {
            var dietToAddToDatabase = new Diet
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    ClientId = Guid.NewGuid(),
                    DietIDs = new Guid[] {dietToAddToDatabase.Id },
                    DeliveryDetails = new DeliveryDetailsDTO
                    {
                        Address = new AddressDTO
                        {
                            Street = "street",
                            BuildingNumber = "buildingNumber",
                            ApartmentNumber = "apartmentNumber",
                            PostCode = "postCode",
                            City = "city"
                        }
                    },
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2)
                },
                dietToAddToDatabase
            };
        }

        public static IEnumerable<object[]> GetAddOrderCommandWithMultipleDietsToAdd()
        {
            var dietsToAdd = new List<Diet>
            {
                new Diet
                {
                    Id = Guid.NewGuid(),
                    IsAvailable = true,
                    Price = 100
                },
                new Diet
                {
                    Id = Guid.NewGuid(),
                    IsAvailable = true,
                    Price = 200
                },
                new Diet
                {
                    Id = Guid.NewGuid(),
                    IsAvailable = true,
                    Price = 300
                }
            };

            yield return new object[]
            {
                new AddOrderCommandWithClientId
                {
                    ClientId = Guid.NewGuid(),
                    DietIDs = new Guid[] { dietsToAdd[0].Id, dietsToAdd[1].Id },
                    DeliveryDetails = new DeliveryDetailsDTO
                    {
                        Address = new AddressDTO
                        {
                            Street = "street",
                            BuildingNumber = "buildingNumber",
                            ApartmentNumber = "apartmentNumber",
                            PostCode = "postCode",
                            City = "city"
                        },
                        PhoneNumber = "1234",
                        CommentForDeliverer = "comment"
                    },
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(10)
                },
                dietsToAdd
            };
        }
    }
}
