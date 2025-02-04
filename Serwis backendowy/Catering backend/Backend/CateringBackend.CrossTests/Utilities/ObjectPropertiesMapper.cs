using System;
using System.Linq;

namespace CateringBackend.CrossTests.Utilities
{
    public static class ObjectPropertiesMapper
    {
        public static void MapProperties<Tin, Tout>(Tin mappFrom, Tout mappTo, string[] skipPropertyNames = null)
        {
            foreach (var property in typeof(Tout).GetProperties())
            {
                if (skipPropertyNames != null && skipPropertyNames.Contains(property.Name)) continue;

                var prop = typeof(Tin).GetProperty(property.Name);
                if (prop == null) continue;
                property.SetValue(mappTo, prop.GetValue(mappFrom));
            }
        }

        public static Tout ConvertObject<Tin, Tout>(Tin inputObject)
        where Tout : new()
        {
            var outputObject = new Tout();
            MapProperties(inputObject, outputObject);
            return outputObject;
        }
    }
}
