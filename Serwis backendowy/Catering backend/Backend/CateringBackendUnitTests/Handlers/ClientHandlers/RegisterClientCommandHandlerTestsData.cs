using System.Collections.Generic;
using CateringBackend.Users.Client.Commands;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public class RegisterClientCommandHandlerTestsData
    {
        public static IEnumerable<object[]> GetValidRegisterClientCommands()
        {
            yield return new object[]
            {
                new RegisterClientCommand
                {
                    Email = "testEmail@gmail.com",
                    LastName = "TestLastName",
                    Name = "TestName",
                    Password = "TestPassword", 
                    PhoneNumber = "+48123456789",
                    Address = new RegisterClientAddress
                    {
                        City = "TestCity",
                        Street = "TestStreet",
                        BuildingNumber = "10A",
                        ApartmentNumber = "1",
                        PostCode = "12-345",
                    }
                }
            };

            yield return new object[]
            {
                new RegisterClientCommand
                {
                    Email = "testEmail2@gmail.com",
                    LastName = "TestLastName2",
                    Name = "TestName2",
                    Password = "TestPassword2",
                    PhoneNumber = "+48987654321",
                    Address = new RegisterClientAddress
                    {
                        City = "TestCity2",
                        Street = "TestStreet2",
                        BuildingNumber = "20A",
                        ApartmentNumber = "2",
                        PostCode = "543-21",
                    }
                }
            };
        }
    }
}
