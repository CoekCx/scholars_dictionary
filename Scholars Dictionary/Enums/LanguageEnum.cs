using System;
using System.Linq;

namespace Scholars_Dictionary.Enums
{
    public enum SupportedLanguages
    {
        [StringValue("en")]
        ENGLISH,
        [StringValue("ru")]
        RUSSIAN,
        [StringValue("es")]
        SPANISH
    }

    // StringValue attribute class
    public class StringValueAttribute : Attribute
    {
        public string Value { get; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }

    public static class EnumExtensions
    {
        public static string GetStringValue(this SupportedLanguages value)
        {
            var stringValueAttribute = (StringValueAttribute)value
                .GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(StringValueAttribute), false)
                .FirstOrDefault();

            return stringValueAttribute?.Value ?? value.ToString();
        }
    }
}
