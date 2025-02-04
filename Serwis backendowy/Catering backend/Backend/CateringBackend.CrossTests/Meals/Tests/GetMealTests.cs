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

namespace CateringBackend.CrossTests.Meals.Tests
{
    public class GetMealTests
    {
        private readonly HttpClient _httpClient;
        private readonly ClientActions ClientActions;
        private readonly ProducerActions ProducerActions;
        private readonly DelivererActions DelivererActions;
        private readonly MealsActions MealsActions;

        public GetMealTests()
        {
            _httpClient = new HttpClient();
            ClientActions = new ClientActions();
            ProducerActions = new ProducerActions();
            MealsActions = new MealsActions();
            DelivererActions = new DelivererActions();
        }

        [Fact]
        public async Task GetMeal_NotLoggedIn_ReturnsUnauthorized()
        {
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            _httpClient.RemoveAuthorization();
            var response = await MealsActions.GetMealById(_httpClient, mealId);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetMeal_DelivererLoggedIn_ReturnsForbidden()
        {
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            await DelivererActions.Authorize(_httpClient);
            var response = await MealsActions.GetMealById(_httpClient, mealId);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetMeal_ProducerLoggedIn_ReturnsOk()
        {
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            await ProducerActions.Authorize(_httpClient);
            var response = await MealsActions.GetMealById(_httpClient, mealId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMeal_ClientLoggedIn_ReturnsOk()
        {
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await MealsActions.GetMealById(_httpClient, mealId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMeal_InvalidId_ReturnsNotFound()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await MealsActions.GetMealById(_httpClient, new Guid().ToString());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
