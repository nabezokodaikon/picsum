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
    }
}
