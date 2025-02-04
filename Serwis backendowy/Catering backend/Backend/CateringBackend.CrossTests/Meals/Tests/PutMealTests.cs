using CateringBackend.CrossTests.Client;
using CateringBackend.CrossTests.Deliverer;
using CateringBackend.CrossTests.Producer;
using CateringBackend.CrossTests.Utilities;
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
    public class PutMealTests
    {
        private readonly HttpClient _httpClient;
        private readonly ClientActions ClientActions;
        private readonly DelivererActions DelivererActions;
        private readonly MealsActions MealsActions;
        private readonly ProducerActions ProducerActions;  

        public PutMealTests()
        {
            _httpClient = new HttpClient();
            ClientActions = new ClientActions();
            MealsActions = new MealsActions();
            DelivererActions = new DelivererActions();
            ProducerActions = new ProducerActions();
        }

        [Fact]
        public async Task PutMeal_NotLoggedIn_ReturnsUnauthorized()
        {
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            _httpClient.RemoveAuthorization();
            var (response, _) = await MealsActions.PutMeal(_httpClient, mealId);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PutMeal_DelivererLoggedIn_ReturnsForbidden()
        {
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            await DelivererActions.Authorize(_httpClient);
            var (response, _) = await MealsActions.PutMeal(_httpClient, mealId);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PutMeal_ProducerLoggedIn_ReturnsOk()
        {
            await ProducerActions.Authorize(_httpClient);
            var (_, meal) = await MealsActions.PostMeal(_httpClient);

            var getResponse = await MealsActions.GetMealByName(_httpClient, meal.Name);
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var mealDb = JsonConvert.DeserializeObject<IEnumerable<Meal>>(getContent).First();

            var (response, newMeal) = await MealsActions.PutMeal(_httpClient, new Guid(mealDb.MealId));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            getResponse = await MealsActions.GetMealByName(_httpClient, newMeal.Name);
            getContent = await getResponse.Content.ReadAsStringAsync();
            mealDb = JsonConvert.DeserializeObject<IEnumerable<Meal>>(getContent).First();

            newMeal.ToExpectedObject().Matches(mealDb);
            meal.ToExpectedObject().DoesNotMatch(mealDb);
        }

        [Fact]
        public async Task PutMeal_ClientLoggedIn_ReturnsForbidden()
        {
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            await ClientActions.RegisterAndLogin(_httpClient);
            var (response, _) = await MealsActions.PutMeal(_httpClient, mealId);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PutMeal_InvalidId_ReturnsNotFound()
        {
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            var (response, _) = await MealsActions.PutMeal(_httpClient, new Guid());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
