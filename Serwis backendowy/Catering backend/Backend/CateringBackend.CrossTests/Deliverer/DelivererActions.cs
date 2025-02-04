using CateringBackend.CrossTests.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CateringBackend.CrossTests.Deliverer
{
    public class DelivererActions
    {
        public async Task Authorize(HttpClient httpClient, bool isValid = true)
        {
            var response = await Login(httpClient, isValid);
            var bearer = await response.Content.ReadAsStringAsync();
            if (bearer.Contains("token"))
            {
                bearer = bearer.Substring(10);
                bearer = bearer.Substring(0, bearer.Length - 2);
            }
            httpClient.RemoveAuthorization();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            httpClient.DefaultRequestHeaders.Add("api-key", bearer);
        }

        public async Task<HttpResponseMessage> Login(HttpClient httpClient, bool isValid = true)
        {
            var loginRequest = DelivererRequestsProvider.PrepareLoginRequest(isValid);
            var body = JsonConvert.SerializeObject(loginRequest).ToStringContent();
            return await httpClient.PostAsync(DelivererUrls.GetLoginUrl(), body);
        }
    }
}
