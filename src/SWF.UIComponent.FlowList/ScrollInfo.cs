using System.Drawing;

namespace SWF.UIComponent.FlowList
{
    public struct ScrollInfo(
        int scrollValue, Size flowListSize, Size itemSize)
    {
        public static readonly ScrollInfo EMPTY
            = new(0, Size.Empty, Size.Empty);

        public int ScrollValue { get; private set; } = scrollValue;
        public Size FlowListSize { get; private set; } = flowListSize;
        public Size ItemSize { get; private set; } = itemSize;
    }
}
