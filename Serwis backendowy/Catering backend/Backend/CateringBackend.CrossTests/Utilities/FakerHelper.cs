using Bogus;

namespace CateringBackend.CrossTests.Utilities
{
    public static class FakerHelper
    {
        public static Faker<T> GetFaker<T>() where T : class
            => new Faker<T>("pl");
    }
}
