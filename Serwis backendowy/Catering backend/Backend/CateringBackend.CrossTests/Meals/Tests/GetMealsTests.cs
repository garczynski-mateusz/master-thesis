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

namespace CateringBackend.CrossTests.Meals.Tests
{
    public class GetMealsTests
    {
        private readonly HttpClient _httpClient;
        private readonly ClientActions ClientActions;
        private readonly ProducerActions ProducerActions;
        private readonly DelivererActions DelivererActions;
        private readonly MealsActions MealsActions;

        public GetMealsTests()
        {
            _httpClient = new HttpClient();
            ClientActions = new ClientActions();
            ProducerActions = new ProducerActions();
            MealsActions = new MealsActions();
            DelivererActions = new DelivererActions();
        }

        [Fact]
        public async Task GetMeals_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await MealsActions.GetMeals(_httpClient);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetMeals_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await MealsActions.GetMeals(_httpClient);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetMeals_ProducerLoggedIn_ReturnsOk()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await MealsActions.GetMeals(_httpClient);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMeals_ClientLoggedIn_ReturnsOk()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await MealsActions.GetMeals(_httpClient);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
