using CateringBackend.CrossTests.Meals;
using CateringBackend.CrossTests.Producer;
using CateringBackend.CrossTests.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CateringBackend.CrossTests.Diets
{
    public class DietsActions
    {
        private readonly ProducerActions ProducerActions;
        private readonly MealsActions MealsActions;
        public DietsActions()
        {
            ProducerActions = new ProducerActions();
            MealsActions = new MealsActions();
        }

        public async Task<HttpResponseMessage> GetDiets(HttpClient httpClient)
        {
            return await httpClient.GetAsync(DietsUrls.GetDietsUrl());
        }

        public async Task<IEnumerable<Guid>> GetDietsIds(HttpClient httpClient)
        {
            var getResponse = await GetDiets(httpClient);
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var diets = JsonConvert.DeserializeObject<IEnumerable<Diet>>(getContent);
            return diets.Select(x => x.Id).ToList();
        }

        public async Task<(HttpResponseMessage, string)> PostDietWithMeals(HttpClient httpClient, bool isValid = true)
        {
            await ProducerActions.Authorize(httpClient);
            var mealId = await MealsActions.PostAndGetMealId(httpClient);
            return await PostDiet(httpClient, new object[] { mealId });
        }

        public async Task<(HttpResponseMessage, string)> PostDiet(HttpClient httpClient, object[] mealIds, bool isValid = true)
        {
            var postRequest = DietsRequestsProvider.PreparePostDietRequest(mealIds, isValid);
            var body = JsonConvert.SerializeObject(postRequest).ToStringContent();
            return (await httpClient.PostAsync(DietsUrls.GetDietsUrl(), body), postRequest.Name);
        }

        public async Task<HttpResponseMessage> GetDietById(HttpClient httpClient, object dietId)
        {
            return await httpClient.GetAsync(DietsUrls.GetDietUrl(dietId));
        }

        public async Task<HttpResponseMessage> GetDietByName(HttpClient httpClient, string dietName)
        {
            return await httpClient.GetAsync(DietsUrls.GetDietsUrl(dietName));
        }

        public async Task<Guid> PostDietAndReturnId(HttpClient httpClient)
        {
            var (_, dietName) = await PostDietWithMeals(httpClient);
            var getResponse = await GetDietByName(httpClient, dietName);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var diet = JsonConvert.DeserializeObject<IEnumerable<Diet>>(getContent).First();

            return diet.Id;
        }

        public async Task<HttpResponseMessage> DeleteDiet(HttpClient httpClient, object dietId)
        {
            return await httpClient.DeleteAsync(DietsUrls.GetDietUrl(dietId));
        }
    }
}
