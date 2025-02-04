using CateringBackend.CrossTests.Utilities;
using System;

namespace CateringBackend.CrossTests.Deliverer
{
    public static class DelivererUrls
    {
        public static string BaseDelivererUrl = UrlProvider.BaseUrl + "/deliverer";
        public static string GetLoginUrl() => $"{BaseDelivererUrl}/login";
        public static string GetOrdersUrl() => $"{BaseDelivererUrl}/orders";
        public static string GetDeliverOrderUrl(Guid orderId) => $"{BaseDelivererUrl}/orders/{orderId}/deliver";
    }
}
