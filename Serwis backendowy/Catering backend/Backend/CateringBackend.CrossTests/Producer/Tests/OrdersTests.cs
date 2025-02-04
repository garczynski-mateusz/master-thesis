using CateringBackend.CrossTests.Client;
using CateringBackend.CrossTests.Deliverer;
using CateringBackend.CrossTests.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackend.CrossTests.Producer.Tests
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
        public async Task GetOrders_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await _httpClient.GetAsync(ProducerUrls.GetOrdersUrl());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetOrders_ClientLoggedIn_ReturnsForbidden()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await _httpClient.GetAsync(ProducerUrls.GetOrdersUrl());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetOrders_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await _httpClient.GetAsync(ProducerUrls.GetOrdersUrl());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetOrders_ProducerLoggedIn_ReturnsOk()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await _httpClient.GetAsync(ProducerUrls.GetOrdersUrl());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetComplaints_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await _httpClient.GetAsync(ProducerUrls.GetComplaintsUrl());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetComplaints_ClientLoggedIn_ReturnsForbidden()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await _httpClient.GetAsync(ProducerUrls.GetComplaintsUrl());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetComplaints_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await _httpClient.GetAsync(ProducerUrls.GetComplaintsUrl());
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetComplaints_ProducertLoggedIn_ReturnsOk()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await _httpClient.GetAsync(ProducerUrls.GetComplaintsUrl());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostOrderComplete_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await _httpClient.PostAsync(ProducerUrls.GetOrderCompleteUrl(new Guid()), null);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PostOrderComplete_ClientLoggedIn_ReturnsForbidden()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await _httpClient.PostAsync(ProducerUrls.GetOrderCompleteUrl(new Guid()), null);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PostOrderComplete_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await _httpClient.PostAsync(ProducerUrls.GetOrderCompleteUrl(new Guid()), null);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PostOrderComplete_InvalidOrderId_ReturnsNotFound()
        {
            await ProducerActions.Authorize(_httpClient);
            var response = await _httpClient.PostAsync(ProducerUrls.GetOrderCompleteUrl(new Guid()), null);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostOrderComplete_OrderNotToRealized_ReturnsNotFound()
        {
            var orderId = await ClientActions.CreatePaidOrder(_httpClient);

            await ProducerActions.Authorize(_httpClient);
            var response = await _httpClient.PostAsync(ProducerUrls.GetOrderCompleteUrl(orderId), null);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostComplaintAnswer_NotLoggedIn_ReturnsUnauthorized()
        {
            var response = await _httpClient.PostAsync(ProducerUrls.GetAnswerComplaintUrl(new Guid()), null);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PostComplaintAnswer_ClientLoggedIn_ReturnsForbidden()
        {
            await ClientActions.RegisterAndLogin(_httpClient);
            var response = await _httpClient.PostAsync(ProducerUrls.GetAnswerComplaintUrl(new Guid()), null);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PostComplaintAnswer_DelivererLoggedIn_ReturnsForbidden()
        {
            await DelivererActions.Authorize(_httpClient);
            var response = await _httpClient.PostAsync(ProducerUrls.GetAnswerComplaintUrl(new Guid()), null);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task PostComplaintAnswer_InvalidComplaintId_ReturnsNotFound()
        {
            await ProducerActions.Authorize(_httpClient);
            var answerRequest = ProducerRequestsProvider.PrepareAnswerComplaintRequest();
            var postBody = JsonConvert.SerializeObject(answerRequest).ToStringContent();
            var response = await _httpClient.PostAsync(ProducerUrls.GetAnswerComplaintUrl(new Guid()), postBody);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
