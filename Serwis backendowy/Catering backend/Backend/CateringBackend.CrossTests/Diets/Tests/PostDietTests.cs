using CateringBackend.CrossTests.Client;
using CateringBackend.CrossTests.Deliverer;
using CateringBackend.CrossTests.Producer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackend.CrossTests.Diets.Tests
{
    public class PostDietTests
    {
        private readonly HttpClient _httpClient;
        private readonly DietsActions DietsActions;
        private readonly ClientActions ClientActions;
        private readonly DelivererActions DelivererActions;

        public PostDietTests()
        {
            _httpClient = new HttpClient();
            DietsActions = new DietsActions();
            ClientActions = new ClientActions();
            DelivererActions = new DelivererActions();
        }

        [Fact]
        public async Task PostDiet_NotLoggedIn_ReturnsUnauthorized()
        {
            var (response, _) = await DietsActions.PostDiet(_httpClient, Array.Empty<object>());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PostDiet_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var (response, _) = await DietsActions.PostDiet(_httpClient, Array.Empty<object>());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PostDiet_ProducerLoggedIn_ReturnsCreated()
        {
            var (response, dietName) = await DietsActions.PostDietWithMeals(_httpClient);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var getResponse = await DietsActions.GetDietByName(_httpClient, dietName);
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var dietDb = JsonConvert.DeserializeObject<IEnumerable<Diet>>(getContent).First();
            Assert.NotNull(dietDb);
        }

        [Fact]
        public async Task PostDiet_ClientLoggedIn_ReturnsForbidden()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var (response, _) = await DietsActions.PostDiet(_httpClient, Array.Empty<object>());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }
    }
}
