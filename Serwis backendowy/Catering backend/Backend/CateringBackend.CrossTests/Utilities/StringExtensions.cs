using System.Net.Http;
using System.Text;

namespace CateringBackend.CrossTests.Utilities
{
    public static class StringExtensions
    {
        public static StringContent ToStringContent(this string content)
        {
            return new StringContent(content, Encoding.UTF8, "application/json");
        }
    }
}
