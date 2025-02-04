using CateringBackend.CrossTests.Client;
using CateringBackend.CrossTests.Deliverer;
using CateringBackend.CrossTests.Meals;
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

namespace CateringBackend.CrossTests.Diets.Tests
{
    public class PutDietTests
    {
        private readonly HttpClient _httpClient;
        private readonly ClientActions ClientActions;
        private readonly DelivererActions DelivererActions;
        private readonly DietsActions DietsActions;
        private readonly ProducerActions ProducerActions;
        private readonly MealsActions MealsActions;

        public PutDietTests()
        {
            _httpClient = new HttpClient();
            ClientActions = new ClientActions();
            DelivererActions = new DelivererActions();
            DietsActions = new DietsActions();
            MealsActions = new MealsActions();
            ProducerActions = new ProducerActions();
        }

        [Fact]
        public async Task PutDiet_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await _httpClient.PutAsync(DietsUrls.GetDietUrl(new Guid()), null);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PutDiet_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await _httpClient.PutAsync(DietsUrls.GetDietUrl(new Guid()), null);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PutDiet_ClientLoggedIn_ReturnsForbidden()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await _httpClient.PutAsync(DietsUrls.GetDietUrl(new Guid()), null);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PutDiet_ProducerLoggedIn_ReturnsOk()
        {
            await ProducerActions.Authorize(_httpClient);
            var dietId = await DietsActions.PostDietAndReturnId(_httpClient);
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            var putRequest = DietsRequestsProvider.PreparePostDietRequest(new object[] { mealId });
            var putBody = JsonConvert.SerializeObject(putRequest).ToStringContent();
            var response = await _httpClient.PutAsync(DietsUrls.GetDietUrl(dietId), putBody);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PutDiet_InvalidDietId_ReturnsNotFound()
        {
            await ProducerActions.Authorize(_httpClient);
            var mealId = await MealsActions.PostAndGetMealId(_httpClient);

            var putRequest = DietsRequestsProvider.PreparePostDietRequest(new object[] { mealId });
            var putBody = JsonConvert.SerializeObject(putRequest).ToStringContent();
            var response = await _httpClient.PutAsync(DietsUrls.GetDietUrl(new Guid()), putBody);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
