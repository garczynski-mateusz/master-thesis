using CateringBackend.CrossTests.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CateringBackend.CrossTests.Producer
{
    public class ProducerActions
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
            var loginRequest = ProducerRequestsProvider.PrepareLoginRequest(isValid);
            var body = JsonConvert.SerializeObject(loginRequest).ToStringContent();
            return await httpClient.PostAsync(ProducerUrls.GetLoginUrl(), body);
        }

        public async Task<IEnumerable<Guid>> GetComplaintIds(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(ProducerUrls.GetComplaintsUrl());
            var content = await response.Content.ReadAsStringAsync();
            var complaintIds = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(content);
            return complaintIds.Select(x => (Guid)x.Id);
        }
    }
}
