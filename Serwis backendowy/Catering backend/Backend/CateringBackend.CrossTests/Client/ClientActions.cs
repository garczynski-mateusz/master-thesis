using CateringBackend.CrossTests.Client.Requests;
using CateringBackend.CrossTests.Diets;
using CateringBackend.CrossTests.Meals;
using CateringBackend.CrossTests.Producer;
using CateringBackend.CrossTests.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CateringBackend.CrossTests.Client
{
    public class ClientActions
    {
        private readonly DietsActions DietsActions;
        private readonly ProducerActions ProducerActions;
        public ClientActions()
        {
            ProducerActions = new ProducerActions();
            DietsActions = new DietsActions();
        }

        public async Task<HttpResponseMessage> Register(HttpClient httpClient, RegisterRequest registerRequest)
        {
            var body = JsonConvert.SerializeObject(registerRequest).ToStringContent();
            return await httpClient.PostAsync(ClientUrls.GetRegisterUrl(), body);
        }

        public async Task<HttpResponseMessage> Login(HttpClient httpClient, LoginRequest request)
        {
            var body = JsonConvert.SerializeObject(request).ToStringContent();
            return await httpClient.PostAsync(ClientUrls.GetLoginUrl(), body);
        }

        public async Task<RegisterRequest> RegisterAndLogin(HttpClient httpClient, bool isValid = true)
        {
            var registerRequest = ClientRequestsProvider.PrepareRegisterRequest();
            await Register(httpClient, registerRequest);

            var loginRequest = ClientRequestsProvider.PrepareLoginRequest(registerRequest, isValid);
            var response = await Login(httpClient, loginRequest);

            var bearer = await response.Content.ReadAsStringAsync();
            if (bearer.Contains("token"))
            {
                bearer = bearer.Substring(10);
                bearer = bearer.Substring(0, bearer.Length - 2);
            }
            httpClient.RemoveAuthorization();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            httpClient.DefaultRequestHeaders.Add("api-key", bearer);
            return registerRequest;
        }

        public async Task<HttpResponseMessage> GetOrders(HttpClient httpClient)
        {
            return await httpClient.GetAsync(ClientUrls.GetOrdersUrl());
        }

        public async Task<(HttpResponseMessage, Guid[] dietIds)> CreateOrder(HttpClient httpClient, bool isValid = true)
        {
            var dietId = await DietsActions.PostDietAndReturnId(httpClient);

            var request = ClientRequestsProvider.PrepareOrdersRequest(new Guid[] { dietId }, isValid);
            var body = JsonConvert.SerializeObject(request).ToStringContent();
            await RegisterAndLogin(httpClient);
            return (await httpClient.PostAsync(ClientUrls.GetOrdersUrl(), body), new Guid[] { dietId });
        }

        public async Task<Guid> CreateOrderAndReturnId(HttpClient httpClient)
        {
            await ProducerActions.Authorize(httpClient);

            var (postResponse, dietIds) = await CreateOrder(httpClient);
            var ordersResponse = await GetOrders(httpClient);
            var ordersContent = await ordersResponse.Content.ReadAsStringAsync();
            var orderIds = JsonConvert.DeserializeObject<IEnumerable<Order>>(ordersContent);
            return orderIds.SingleOrDefault(x => x.Diets.Select(x => x.DietId).SequenceEqual(dietIds)).Id;
        }

        public async Task<HttpResponseMessage> SendComplain(HttpClient httpClient, object orderId)
        {
            var request = ClientRequestsProvider.PrepareComplainRequest();
            var body = JsonConvert.SerializeObject(request).ToStringContent();
            return await httpClient.PostAsync(ClientUrls.GetOrdersComplainUrl(orderId), body);
        }

        public async Task<HttpResponseMessage> PayOrder(HttpClient httpClient, object orderId)
        {
            return await httpClient.PostAsync(ClientUrls.GetOrdersPayUrl(orderId), null);
        }

        public async Task<Guid> CreatePaidOrder(HttpClient httpClient)
        {
            var orderId = await CreateOrderAndReturnId(httpClient);
            await PayOrder(httpClient, orderId);
            return orderId;
        }
    }
}
