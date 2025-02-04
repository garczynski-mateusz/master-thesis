using CateringBackend.CrossTests.Client.Requests;
using CateringBackend.CrossTests.Utilities;
using System;
using System.Linq;

namespace CateringBackend.CrossTests.Client
{
    public static class ClientRequestsProvider
    {
        public static RegisterRequest PrepareRegisterRequest(bool isValid = true)
        {
            var fakerRegister = FakerHelper.GetFaker<RegisterRequest>()
                .RuleFor(x => x.Name, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Password, f => f.Internet.Password())
                .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber("#########"));
            var registerRequest = fakerRegister.Generate();

            if (isValid)
            {
                var address = PrepareAddress();
                registerRequest.Address = address;
            }
            return registerRequest;
        }

        public static LoginRequest PrepareLoginRequest(RegisterRequest registerRequest, bool isValid = true)
        {
            var loginFaker = FakerHelper.GetFaker<LoginRequest>()
                .RuleFor(x => x.Password, f => f.Internet.Password(5));

            var loginRequest = loginFaker.Generate();
            loginRequest.Email = registerRequest.Email;
            if (isValid)
                loginRequest.Password = registerRequest.Password;
            return loginRequest;
        }

        public static EditClientRequest PrepareEditClientRequest(bool isValid = true)
        {
            var fakerEdit = FakerHelper.GetFaker<EditClientRequest>()
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Name, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Password, f => f.Internet.Password())
                .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber("#########"));
            var editRequest = fakerEdit.Generate();
            editRequest.Address = PrepareAddress();
            if (!isValid)
                editRequest.Password = null;
            return editRequest;
        }

        public static ClientAddress PrepareAddress()
        {
            var fakeAddress = FakerHelper.GetFaker<ClientAddress>()
                    .RuleFor(x => x.Street, f => f.Address.StreetName())
                    .RuleFor(x => x.BuildingNumber, f => f.Random.Number(20).ToString())
                    .RuleFor(x => x.ApartmentNumber, f => f.Random.Number(20).ToString())
                    .RuleFor(x => x.PostCode, f => f.Address.ZipCode("##-###"))
                    .RuleFor(x => x.City, f => f.Address.City());

            return fakeAddress.Generate();
        }

        public static PostOrdersRequest PrepareOrdersRequest( Guid[] dietIds, bool isValid = true)
        {
            var fakerOrders = FakerHelper.GetFaker<PostOrdersRequest>()
                .RuleFor(x => x.DietIDs, f => dietIds.Select(x => x).ToArray())
                .RuleFor(x => x.StartDate, f => f.Date.Between(new DateTime(2023, 1, 1), new DateTime(2023, 2, 1)))
                .RuleFor(x => x.EndDate, f => f.Date.Between(new DateTime(2023, 2, 2), new DateTime(2023, 3, 1)));

            var orderRequest = fakerOrders.Generate();

            if (isValid)
            {
                orderRequest.DeliveryDetails = FakerHelper.GetFaker<DeliveryDetails>()
                    .RuleFor(x => x.Address, f => PrepareAddress())
                    .RuleFor(x => x.CommentForDeliverer, f => f.Lorem.Sentence(5))
                    .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber("#########"))
                    .Generate();
            }

            return orderRequest;
        }

        public static PostComplainRequest PrepareComplainRequest()
        {
            var fakerComplain = FakerHelper.GetFaker<PostComplainRequest>()
                .RuleFor(x => x.Complain_Description, f => f.Lorem.Sentence(5));

            return fakerComplain.Generate();
        }
    }
}
