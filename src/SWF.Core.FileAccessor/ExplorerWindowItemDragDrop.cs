using System.Windows.Automation;

namespace SWF.Core.FileAccessor
{
    internal static class ExplorerWindowItemDragDrop
    {
        public static string GetExplorerPathAtCursor(int x, int y)
        {
            var element = AutomationElement.FromPoint(new System.Windows.Point(x, y));
            if (element == null)
            {
                return string.Empty;
            }

            if (element.Current.ControlType == ControlType.ListItem
                || element.Current.ControlType == ControlType.Edit)
            {
                if (element.TryGetCurrentPattern(ValuePattern.Pattern, out var pattern))
                {
                    var valuePattern = pattern as ValuePattern;
                    if (valuePattern == null)
                    {
                        return string.Empty;
                    }

                    return valuePattern.Current.Value;
                }

                return element.Current.Name;
            }

            return string.Empty;
        }
    }
}
