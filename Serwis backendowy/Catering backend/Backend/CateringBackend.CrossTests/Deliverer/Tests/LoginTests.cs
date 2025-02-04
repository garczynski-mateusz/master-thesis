using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackend.CrossTests.Deliverer.Tests
{
    public class LoginTests
    {
        private readonly HttpClient _httpClient;
        private readonly DelivererActions DelivererActions;

        public LoginTests()
        {
            _httpClient = new HttpClient();
            DelivererActions = new DelivererActions();
        }

        [Fact]
        public async void LoginClient_HasCorrectData_ReturnsOK()
        {
            var response = await DelivererActions.Login(_httpClient);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void LoginClient_HasIncorrectPassword_ReturnsBadRequest()
        {
            var response = await DelivererActions.Login(_httpClient, false);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
