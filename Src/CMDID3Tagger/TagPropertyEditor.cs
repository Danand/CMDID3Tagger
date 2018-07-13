using System.Globalization;
using System.Linq;
using System.Reflection;

using CMDID3Tagger.Interfaces;

using TagLib;

namespace CMDID3Tagger
{
    public class TagPropertyEditor : ITagPropertyEditor
    {
        private readonly IOutput output;

        public TagPropertyEditor(IOutput output)
        {
            this.output = output;
        }

        void ITagPropertyEditor.AssignTag(File tagLibFile, string name, string value)
        {
            name = FixTagName(name);

            var tag = tagLibFile.Tag;
            var propertyInfo = GetTagProperty(tag, name);

            if (propertyInfo == default(PropertyInfo))
            {
                output.Write($"Unrecognized tag '{name}' with value '{value}'!");
                return;
            }

            if (propertyInfo.PropertyType == typeof(uint))
                propertyInfo.SetValue(tag, uint.Parse(value));

            else if (propertyInfo.PropertyType == typeof(string))
                propertyInfo.SetValue(tag, value);

            else if (propertyInfo.PropertyType == typeof(string[]))
                propertyInfo.SetValue(tag, new[] { value });

            output.Write($"'{value}' {name} tag added to '{tagLibFile.Name}'.");
        }

        string ITagPropertyEditor.GetTagValue(File tagLibFile, string name)
        {
            name = FixTagName(name);

            var tag = tagLibFile.Tag;

            var propertyInfo = GetTagProperty(tag, name);

            if (propertyInfo == default(PropertyInfo))
            {
                output.Write($"Unrecognized tag '{name}'!");
                return string.Empty;
            }

            var value = propertyInfo.GetValue(tag);

            if (value is uint valueUint)
                return valueUint.ToString(CultureInfo.InvariantCulture);

            if (value is string valueString)
                return valueString;

            if (value is string[] valueArray)
                return valueArray.FirstOrDefault() ?? string.Empty;

            output.Write($"Unrecognized tag '{name}'!");

            return string.Empty;
        }

        private PropertyInfo GetTagProperty(Tag tag, string name)
        {
            return tag
                .GetType()
                .GetProperties()
                .FirstOrDefault(property => property.CanWrite &&
                                            property.Name.ToLowerInvariant() == name);
        }

        private string FixTagName(string name)
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
