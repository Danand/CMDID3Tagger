using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TagLib;

namespace CMDID3Tagger
{
    internal static class TagPropertyWrapper
    {
        public static void AssignTag(File tagLibFile, string name, string value)
        {
            name = FixTagName(name);

            var tag = tagLibFile.Tag;
            var propertyInfo = GetTagProperty(tag, name);

            if (propertyInfo == default(PropertyInfo))
            {
                Console.WriteLine($"Unrecognized tag '{name}' with value '{value}'!");
                return;
            }

            if (propertyInfo.PropertyType == typeof(uint))
                propertyInfo.SetValue(tag, uint.Parse(value));

            else if (propertyInfo.PropertyType == typeof(string))
                propertyInfo.SetValue(tag, value);

            else if (propertyInfo.PropertyType == typeof(string[]))
                propertyInfo.SetValue(tag, new[] { value });

            Console.WriteLine($"'{value}' {name} tag added to '{tagLibFile.Name}'.");
        }

        public static string GetTagValue(File tagLibFile, string name)
        {
            name = FixTagName(name);

            var tag = tagLibFile.Tag;

            var propertyInfo = GetTagProperty(tag, name);

            if (propertyInfo == default(PropertyInfo))
            {
                Console.WriteLine($"Unrecognized tag '{name}'!");
                return string.Empty;
            }

            var value = propertyInfo.GetValue(tag);

            if (value is uint valueUint)
                return valueUint.ToString(CultureInfo.InvariantCulture);

            if (value is string valueString)
                return valueString;

            if (value is string[] valueArray)
                return valueArray.FirstOrDefault() ?? string.Empty;

            Console.WriteLine($"Unrecognized tag '{name}'!");

            return string.Empty;
        }

        private static PropertyInfo GetTagProperty(Tag tag, string name)
        {
            return tag
                .GetType()
                .GetProperties()
                .FirstOrDefault(property => property.CanWrite &&
                                            property.Name.ToLowerInvariant() == name);
        }

        private static string FixTagName(string name)
        {
            switch (name)
            {
                case "artist":
                    return "performers";
                default:
                    return name;
            }
        }
    }
}
