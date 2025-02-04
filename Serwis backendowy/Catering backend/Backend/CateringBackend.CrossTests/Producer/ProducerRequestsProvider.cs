using CateringBackend.CrossTests.Client.Requests;
using CateringBackend.CrossTests.Producer.Requests;

namespace CateringBackend.CrossTests.Producer
{
    public static class ProducerRequestsProvider
    {
        public static LoginRequest PrepareLoginRequest(bool isValid = true)
        {
            var request =  new LoginRequest()
            {
                Email = "producent@producent.pl",
                Password = "Producent123!"
            };
            if (!isValid)
                request.Password = string.Empty;

            return request;
        }

        public static AnswerComplaintRequest PrepareAnswerComplaintRequest()
        {
            var request = new AnswerComplaintRequest()
            {
                Compliant_answer = "Ok"
            };
            return request;
        }
    }
}
