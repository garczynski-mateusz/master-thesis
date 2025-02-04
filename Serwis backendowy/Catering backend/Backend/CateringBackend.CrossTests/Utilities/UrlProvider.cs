
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace CateringBackend.CrossTests.Utilities
{
    public static class UrlProvider
    {
        public static string BaseUrl => LazyBaseUrl.Value;

        private static readonly Lazy<string> LazyBaseUrl = new(GetBaseUrlFromConfiguration, LazyThreadSafetyMode.ExecutionAndPublication);

        private static string GetBaseUrlFromConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            
            var config = configurationBuilder.AddJsonFile("appsettings.crossTeamTests.json")
                .AddEnvironmentVariables()
                .Build();

            return config["CateringUrl"];
        }        
    }
}
