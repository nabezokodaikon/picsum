using WinApi;

namespace SWF.Core.Base
{
    public sealed class NaturalStringComparer
        : IComparer<string?>
    {
        public static IComparer<string?> Windows { get; } = new NaturalStringComparer();

        private NaturalStringComparer()
        {
        }

        public int Compare(string? x, string? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return WinApiMembers.StrCmpLogicalW(x, y);
        }
    }
}
