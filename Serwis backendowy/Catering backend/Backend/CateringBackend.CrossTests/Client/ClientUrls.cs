using CateringBackend.CrossTests.Utilities;

namespace CateringBackend.CrossTests.Client
{
    public static class ClientUrls
    {
        public static string BaseClientUrl = UrlProvider.BaseUrl + "/client";
        public static string GetRegisterUrl() => $"{BaseClientUrl}/register";
        public static string GetLoginUrl() => $"{BaseClientUrl}/login";
        public static string GetOrdersUrl(string status = null)
        { if (!string.IsNullOrWhiteSpace(status))
                return $"{BaseClientUrl}/orders?Status={status}";
            return $"{BaseClientUrl}/orders";
        }
        public static string GetAccountUrl() => $"{BaseClientUrl}/account";
        public static string GetOrdersComplainUrl(object orderId) => $"{BaseClientUrl}/orders/{orderId}/complain";
        public static string GetOrdersPayUrl(object orderId) => $"{BaseClientUrl}/orders/{orderId}/pay";
    }
}
