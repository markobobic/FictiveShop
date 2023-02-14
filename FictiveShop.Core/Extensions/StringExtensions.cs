using Ardalis.GuardClauses;

namespace FictiveShop.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source == null || !source.Any();

        public static string ExtractNumber(this string source)
        {
            if (source.IsNullOrEmpty()) Guard.Against.Null(source, nameof(source), "Telephone number is null");
            return new string(source.ToCharArray().Where(x => char.IsNumber(x)).ToArray());
        }

        public static int ToInt(this string value)
        {
            int number;

            int.TryParse(value, out number);

            return number;
        }
    }
}