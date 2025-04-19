namespace SWF.Core.Base
{
    public static class StringUtil
    {
        public static bool Compare(string a, string b)
        {
            ArgumentNullException.ThrowIfNull(a, nameof(a));
            ArgumentNullException.ThrowIfNull(b, nameof(b));

            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static int? ToInt(string text)
        {
            ArgumentNullException.ThrowIfNull(text, nameof(text));

            if (int.TryParse(text, out var result))
            {
                return result;
            }

            return null;
        }

        public static string ExtractNumbers(string text)
        {
            ArgumentNullException.ThrowIfNull(text, nameof(text));

            var numbers = "";
            foreach (var c in text)
            {
                if (char.IsDigit(c))
                {
                    numbers += c;
                }
            }

            return numbers;
        }
    }
}
