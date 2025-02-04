using CateringBackend.CrossTests.Utilities;

namespace CateringBackend.CrossTests.Meals
{
    public static class MealsUrls
    {
        public static string BaseMealsUrl = UrlProvider.BaseUrl + "/meals";
        public static string GetMealsUrl(string name = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return $"{BaseMealsUrl}?Name={name}";
            }
            return BaseMealsUrl;
        }
        public static string GetMealUrl(object mealId) => $"{BaseMealsUrl}/{mealId}";
    }
}
