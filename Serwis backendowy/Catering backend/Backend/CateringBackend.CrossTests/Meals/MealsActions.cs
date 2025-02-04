using CateringBackend.CrossTests.Meals.Requests;
using CateringBackend.CrossTests.Producer;
using CateringBackend.CrossTests.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CateringBackend.CrossTests.Meals
{
    public class MealsActions
    {
        private readonly ProducerActions ProducerActions;
        public MealsActions()
        {
            ProducerActions = new ProducerActions();
        }

        public  async Task<HttpResponseMessage> GetMeals(HttpClient httpClient)
        {
            return await httpClient.GetAsync(MealsUrls.GetMealsUrl());
        }

        public async Task<IEnumerable<object>> GetMealsIds(HttpClient httpClient)
        {
            var getResponse = await GetMeals(httpClient);
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var diets = JsonConvert.DeserializeObject<IEnumerable<Meal>>(getContent);
            return diets?.Select(x => x.MealId)?.ToList();
        }

        public async Task<Guid> PostAndGetMealId(HttpClient httpClient)
        {
            await ProducerActions.Authorize(httpClient);
            var (postResponse, postMeal) = await PostMeal(httpClient);
            var getResponse = await GetMealByName(httpClient, postMeal.Name);
            var content = await getResponse.Content.ReadAsStringAsync();
            var meal = JsonConvert.DeserializeObject<IEnumerable<Meal>>(content).First();

            return new Guid(meal.MealId);
        }

        public async Task<HttpResponseMessage> GetMealById(HttpClient httpClient, object mealId)
        {
            return await httpClient.GetAsync(MealsUrls.GetMealUrl(mealId));
        }

        public async Task<HttpResponseMessage> GetMealByName(HttpClient httpClient, string mealName)
        {
            return await httpClient.GetAsync(MealsUrls.GetMealsUrl(mealName));
        }


        public async Task<(HttpResponseMessage, Meal)> PutMeal(HttpClient httpClient, Guid? mealId)
        {
            var putRequest = MealsRequestsProvider.PrepareMeal();
            putRequest.MealId = mealId.ToString();

            var putBody = JsonConvert.SerializeObject(putRequest).ToStringContent();
            var putResponse = await httpClient.PutAsync(MealsUrls.GetMealUrl(mealId), putBody);
            return (putResponse, putRequest);
        }

        public async Task<(HttpResponseMessage, Meal)> PostMeal(HttpClient httpClient, bool isValid = true)
        {
            var meal = MealsRequestsProvider.PrepareMeal(isValid);
            var postRequest = ObjectPropertiesMapper.ConvertObject<Meal, PostMealRequest>(meal);
            var body = JsonConvert.SerializeObject(postRequest).ToStringContent();
            return (await httpClient.PostAsync(MealsUrls.GetMealsUrl(), body), meal);
        }

        public async Task<HttpResponseMessage> DeleteMeal(HttpClient httpClient, object mealId)
        {
            return await httpClient.DeleteAsync(MealsUrls.GetMealUrl(mealId));
        }
    }
}
