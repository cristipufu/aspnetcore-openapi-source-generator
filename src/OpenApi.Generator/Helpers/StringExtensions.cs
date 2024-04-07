using System;
using System.Linq;

namespace OpenApi.Generator
{
    internal static class StringExtensions
    {
        public static string ToPascalCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            text = text.ToLowerInvariant(); 
            var words = text.Split(new[] { '-', '_', ' ' }, StringSplitOptions.RemoveEmptyEntries); // Split by common word delimiters
            var result = string.Concat(words.Select(word => char.ToUpperInvariant(word[0]) + word.Substring(1)));
            return result;
        }
    }
}
