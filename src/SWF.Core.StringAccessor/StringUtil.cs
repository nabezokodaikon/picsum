namespace SWF.Core.StringAccessor
{
    public static class StringUtil
    {
        public static bool CompareFilePath(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return true;
            }

            if (string.IsNullOrEmpty(a))
            {
                return false;
            }

            if (string.IsNullOrEmpty(b))
            {
                return false;
            }

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
