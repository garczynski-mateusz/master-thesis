using CateringBackend.CrossTests.Client.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace CateringBackend.CrossTests.Deliverer
{
    public static class DelivererRequestsProvider
    {
        public static LoginRequest PrepareLoginRequest(bool isValid = true)
        {
            var request =  new LoginRequest()
            {
                Email = "dostawca@dostawca.pl",
                Password = "Dostawca123!"
            };

            if (!isValid)
                request.Password = string.Empty;

            return request;
        }
    }
}
