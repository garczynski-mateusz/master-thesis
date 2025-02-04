using CateringBackend.CrossTests.Utilities;

namespace CateringBackend.CrossTests.Diets
{
    public static class DietsUrls
    {
        public static string BaseDietsUrl = UrlProvider.BaseUrl + "/diets";
        public static string GetDietsUrl(string name = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
                return $"{BaseDietsUrl}?Name={name}";
            return BaseDietsUrl;
        }
        public static string GetDietUrl(object dietId) => $"{BaseDietsUrl}/{dietId}";
    }
}
