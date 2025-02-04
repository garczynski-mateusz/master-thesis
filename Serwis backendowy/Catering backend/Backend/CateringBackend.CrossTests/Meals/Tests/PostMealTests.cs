using CateringBackend.CrossTests.Client;
using CateringBackend.CrossTests.Deliverer;
using CateringBackend.CrossTests.Producer;
using ExpectedObjects;
using Newtonsoft.Json;
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
    public class PostMealTests
    {
        private readonly HttpClient _httpClient;
        private readonly ClientActions ClientActions;
        private readonly ProducerActions ProducerActions;
        private readonly DelivererActions DelivererActions;
        private readonly MealsActions MealsActions;

        public PostMealTests()
        {
            _httpClient = new HttpClient();
            ClientActions = new ClientActions();
            ProducerActions = new ProducerActions();
            MealsActions = new MealsActions();
            DelivererActions = new DelivererActions();
        }

        [Fact]
        public async Task PostMeal_NotLoggedIn_ReturnsUnauthorized()
        {
            var (response, _) = await MealsActions.PostMeal(_httpClient);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PostMeal_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var (response, _) = await MealsActions.PostMeal(_httpClient);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PostMeal_ProducerLoggedIn_ReturnsOk()
        {
            await ProducerActions.Authorize(_httpClient);
            var (response, meal) = await MealsActions.PostMeal(_httpClient);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var getResponse = await MealsActions.GetMealByName(_httpClient, meal.Name);
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var mealDb = JsonConvert.DeserializeObject<IEnumerable<Meal>>(getContent).First();
            meal.ToExpectedObject().Matches(mealDb);
        }

        [Fact]
        public async Task PostMeal_ClientLoggedIn_ReturnsForbidden()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var (response, _) = await MealsActions.PostMeal(_httpClient);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }
    }
}
