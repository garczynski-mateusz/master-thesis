using CateringBackend.CrossTests.Client.Requests;
using CateringBackend.CrossTests.Client.Responses;
using CateringBackend.CrossTests.Utilities;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using ExpectedObjects;
using CateringBackend.CrossTests.Producer;
using CateringBackend.CrossTests.Deliverer;

namespace CateringBackend.CrossTests.Client.Tests
{
    public class AccountTests
    {

        private readonly HttpClient _httpClient;
        private readonly ClientActions ClientActions;
        private readonly ProducerActions ProducerActions;
        private readonly DelivererActions DelivererActions;

        public AccountTests()
        {
            _httpClient = new HttpClient();
            ClientActions = new ClientActions();
            ProducerActions = new ProducerActions();
            DelivererActions = new DelivererActions();
        }

        [Fact]
        public async Task GetClientDetails_IsLoggedIn_ReturnsOK()
        {
            var clientData = await ClientActions.RegisterAndLogin(_httpClient);
            var getResponse = await _httpClient.GetAsync(ClientUrls.GetAccountUrl());
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var responseContent = await getResponse.Content.ReadAsStringAsync();
            var clientGetData = JsonConvert.DeserializeObject<GetClientDetailsResponse>(responseContent);
            var expectedData = ObjectPropertiesMapper.ConvertObject<RegisterRequest, GetClientDetailsResponse>(clientData);
            clientGetData.ToExpectedObject().ShouldEqual(expectedData);
        }

        [Fact]
        public async Task GetClientDetails_ProducerLoggedIn_ReturnsForbidden()
        {
            await ProducerActions.Authorize(_httpClient);
            var getResponse = await _httpClient.GetAsync(ClientUrls.GetAccountUrl());
            Assert.True(getResponse.StatusCode == HttpStatusCode.Unauthorized || getResponse.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetClientDetails_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var getResponse = await _httpClient.GetAsync(ClientUrls.GetAccountUrl());
            Assert.True(getResponse.StatusCode == HttpStatusCode.Unauthorized || getResponse.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetClientDetails_NotLoggedIn_ReturnsUnauthorized()
        {
            var getResponse = await _httpClient.GetAsync(ClientUrls.GetAccountUrl());
            Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        }

        [Fact]
        public async void UpdateClientDetails_ProvidedCompleteData_ReturnsOK()
        {
            var clientData = await ClientActions.RegisterAndLogin(_httpClient);
            var request = ClientRequestsProvider.PrepareEditClientRequest();
            var body = JsonConvert.SerializeObject(request).ToStringContent();
            var response = await _httpClient.PutAsync(ClientUrls.GetAccountUrl(), body);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var getResponse = await _httpClient.GetAsync(ClientUrls.GetAccountUrl());
            var responseContent = await getResponse.Content.ReadAsStringAsync();
            var clientGetData = JsonConvert.DeserializeObject<GetClientDetailsResponse>(responseContent);
            var expectedData = ObjectPropertiesMapper.ConvertObject<EditClientRequest, GetClientDetailsResponse>(request);
            clientGetData.ToExpectedObject().ShouldEqual(expectedData);
        }

        [Fact]
        public async void UpdateClientDetails_NotLoggedIn_ReturnsUnauthorized()
        {
            var request = ClientRequestsProvider.PrepareEditClientRequest();
            var body = JsonConvert.SerializeObject(request).ToStringContent();
            var response = await _httpClient.PutAsync(ClientUrls.GetAccountUrl(), body);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void UpdateClientDetails_ProducerLoggedIn_ReturnsForbidden()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await _httpClient.PutAsync(ClientUrls.GetAccountUrl(), null);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async void UpdateClientDetails_DelivererLoggedIn_ReturnsForbidden()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await _httpClient.PutAsync(ClientUrls.GetAccountUrl(), null);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }
    }
}
