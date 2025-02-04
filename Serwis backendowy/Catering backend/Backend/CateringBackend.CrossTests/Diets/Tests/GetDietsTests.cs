using CateringBackend.CrossTests.Client;
using CateringBackend.CrossTests.Deliverer;
using CateringBackend.CrossTests.Producer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackend.CrossTests.Diets.Tests
{
    public class GetDietsTests
    {
        private readonly HttpClient _httpClient;
        private readonly DietsActions DietsActions;
        private readonly ClientActions ClientActions;
        private readonly ProducerActions ProducerActions;
        private readonly DelivererActions DelivererActions;

        public GetDietsTests()
        {
            _httpClient = new HttpClient();
            DietsActions = new DietsActions();
            ClientActions = new ClientActions();
            ProducerActions = new ProducerActions();
            DelivererActions = new DelivererActions();
        }

        [Fact]
        public async Task GetDiets_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await DietsActions.GetDiets(_httpClient);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetDiets_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await DietsActions.GetDiets(_httpClient);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetDiets_ProducerLoggedIn_ReturnsOk()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await DietsActions.GetDiets(_httpClient);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetDiets_ClientLoggedIn_ReturnsOk()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await DietsActions.GetDiets(_httpClient);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
