namespace Scholars_Dictionary.Enums
{
    public static class EnumHelper
    {
        public static string WordTypeString(WordType value)
        {
            string stringValue = value.ToString();
            return char.ToUpper(stringValue[0]) + stringValue.Substring(1).ToLower();
        }
    }
}