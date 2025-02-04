using CateringBackend.CrossTests.Utilities;
using System;

namespace CateringBackend.CrossTests.Producer
{
    public static class ProducerUrls
    {
        public static string BaseProducerUrl = UrlProvider.BaseUrl + "/producer";
        public static string GetLoginUrl() => $"{BaseProducerUrl}/login";
        public static string GetOrdersUrl() => $"{BaseProducerUrl}/orders";
        public static string GetComplaintsUrl() => $"{BaseProducerUrl}/orders/complaints";
        public static string GetAnswerComplaintUrl(Guid complaintId) => $"{BaseProducerUrl}/orders/{complaintId}/answer-complaint";
        public static string GetOrderCompleteUrl(Guid orderId) => $"{BaseProducerUrl}/orders/{orderId}/complete";
    }
}
