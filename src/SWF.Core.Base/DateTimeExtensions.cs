namespace SWF.Core.Base
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime EMPTY = DateTime.MinValue;

        public static bool IsEmpty(this DateTime date)
        {
            return date == EMPTY;
        }
    }
}
