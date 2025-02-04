using CateringBackend.CrossTests.Client.Requests;

namespace CateringBackend.CrossTests.Client.Responses
{
    public class GetClientDetailsResponse
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public ClientAddress Address { get; set; }
    }
}
