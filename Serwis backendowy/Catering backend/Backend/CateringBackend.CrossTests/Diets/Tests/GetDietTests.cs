using CateringBackend.CrossTests.Client;
using CateringBackend.CrossTests.Deliverer;
using CateringBackend.CrossTests.Producer;
using CateringBackend.CrossTests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackend.CrossTests.Diets.Tests
{
    public class GetDietTests
    {
        private readonly HttpClient _httpClient;
        private readonly ClientActions ClientActions;
        private readonly DietsActions DietsActions;
        private readonly ProducerActions ProducerActions;
        private readonly DelivererActions DelivererActions;

        public GetDietTests()
        {
            _httpClient = new HttpClient();
            DietsActions = new DietsActions();
            ClientActions = new ClientActions();
            ProducerActions = new ProducerActions();
            DelivererActions = new DelivererActions();
        }

        [Fact]
        public async Task GetDiet_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await DietsActions.GetDietById(_httpClient, new Guid().ToString());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetDiet_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await DietsActions.GetDietById(_httpClient, new Guid().ToString());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetDiet_InvalidId_ReturnsNotFound()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await DietsActions.GetDietById(_httpClient, new Guid().ToString());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetDiet_ProducerLoggedIn_ReturnsOk()
        {
            var dietId = await DietsActions.PostDietAndReturnId(_httpClient);
            var response = await DietsActions.GetDietById(_httpClient, dietId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetDiet_ClientLoggedIn_ReturnsOk()
        {
            var dietId = await DietsActions.PostDietAndReturnId(_httpClient);

            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await DietsActions.GetDietById(_httpClient, dietId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
