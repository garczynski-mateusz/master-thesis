namespace CateringBackend.CrossTests.Client.Requests
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public ClientAddress Address { get; set; }
    }

    public class ClientAddress
    {
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
    }
}
