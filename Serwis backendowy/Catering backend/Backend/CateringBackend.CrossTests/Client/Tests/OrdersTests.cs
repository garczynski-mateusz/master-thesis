using System;
using CateringBackend.CrossTests.Deliverer;
using CateringBackend.CrossTests.Producer;
using CateringBackend.CrossTests.Utilities;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;


namespace CateringBackend.CrossTests.Client.Tests
{
    public class OrdersTests
    {
        private readonly HttpClient _httpClient;
        private readonly ClientActions ClientActions;
        private readonly ProducerActions ProducerActions;
        private readonly DelivererActions DelivererActions;

        public OrdersTests()
        {
            _httpClient = new HttpClient();
            ClientActions = new ClientActions();
            ProducerActions = new ProducerActions();
            DelivererActions = new DelivererActions();
        }

        [Fact]
        public async Task GetOrders_IsLoggedIn_ReturnsOK()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var getResponse = await ClientActions.GetOrders(_httpClient);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var orders = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(getResponse.Content.ReadAsStringAsync().Result);
            Assert.Empty(orders);
        }

        [Fact]
        public async Task GetOrders_NotLoggedIn_ReturnsUnauthorized()
        {
            var getResponse = await ClientActions.GetOrders(_httpClient);
            Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        }

        [Fact]
        public async Task GetOrders_ProducerLoggedIn_ReturnsForbidden()
        {
            await ProducerActions.Authorize(_httpClient);
            var getResponse = await ClientActions.GetOrders(_httpClient);
            Assert.True(getResponse.StatusCode == HttpStatusCode.Unauthorized || getResponse.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetOrders_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var getResponse = await ClientActions.GetOrders(_httpClient);
            Assert.True(getResponse.StatusCode == HttpStatusCode.Unauthorized || getResponse.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task SendOrders_ProvidedCompleteData_ReturnsOK()
        {
            var (postResponse, dietIds) = await ClientActions.CreateOrder(_httpClient);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var getResponse = await ClientActions.GetOrders(_httpClient);
            var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(getResponse.Content.ReadAsStringAsync().Result);
            Assert.NotEmpty(orders.Where(x => x.Diets.Select(x => x.DietId).SequenceEqual(dietIds)));
        }

        [Fact]
        public async Task SendOrders_NotLoggedIn_ReturnsUnauthorized()
        {
            var postResponse = await _httpClient.PostAsync(ClientUrls.GetOrdersUrl(), null);
            Assert.Equal(HttpStatusCode.Unauthorized, postResponse.StatusCode);
        }

        [Fact]
        public async Task SendOrders_ProducerLoggedIn_ReturnsForbidden()
        {
            await ProducerActions.Authorize(_httpClient);
            var postResponse = await _httpClient.PostAsync(ClientUrls.GetOrdersUrl(), null);
            Assert.True(postResponse.StatusCode == HttpStatusCode.Unauthorized || postResponse.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task SendOrders_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var postResponse = await _httpClient.PostAsync(ClientUrls.GetOrdersUrl(), null);
            Assert.True(postResponse.StatusCode == HttpStatusCode.Unauthorized || postResponse.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task SendComplain_CorrectData_ReturnsCreated()
        {
            var orderId = await ClientActions.CreateOrderAndReturnId(_httpClient);

            var response = await ClientActions.SendComplain(_httpClient, orderId);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task SendComplain_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await ClientActions.SendComplain(_httpClient, new Guid().ToString());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SendComplain_ProducerLoggedIn_ReturnsForbidden()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await ClientActions.SendComplain(_httpClient, new Guid().ToString());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task SendComplain_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await ClientActions.SendComplain(_httpClient, new Guid().ToString());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task SendComplain_InvalidOrderId_ReturnsNotFound()
        {
            await ClientActions.CreateOrderAndReturnId(_httpClient);

            var response = await ClientActions.SendComplain(_httpClient, new Guid().ToString());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PayOrder_ValidOrderId_ReturnsCreated()
        {
            var orderId = await ClientActions.CreateOrderAndReturnId(_httpClient);
            var response = await ClientActions.PayOrder(_httpClient, orderId);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var getResponse = await ClientActions.GetOrders(_httpClient);
            var order = JsonConvert.DeserializeObject<IEnumerable<Order>>(getResponse.Content.ReadAsStringAsync().Result).SingleOrDefault(x => x.Id == orderId);
            Assert.NotNull(order);
            Assert.Equal("Paid", order.Status);
        }

        [Fact]
        public async Task PayOrder_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await ClientActions.PayOrder(_httpClient, new Guid().ToString());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PayOrder_ProducerLoggedIn_ReturnsForbidden()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await ClientActions.PayOrder(_httpClient, new Guid().ToString());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PayOrder_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await ClientActions.PayOrder(_httpClient, new Guid().ToString());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PayOrder_InvalidOrderId_ReturnsNotFound()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await ClientActions.PayOrder(_httpClient, new Guid().ToString());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
