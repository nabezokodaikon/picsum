using System.IO;
using System.Windows.Automation;

namespace SWF.Core.FileAccessor
{
    internal sealed class ExplorerDesktopItemDragDrop
    {
        public static string GetExplorerPathAtCursor(int x, int y)
        {
            var element = AutomationElement.FromPoint(new System.Windows.Point(x, y));
            if (element == null)
            {
                return string.Empty;
            }

            if (element.Current.ControlType != ControlType.ListItem)
            {
                return string.Empty;
            }

            if (!element.TryGetCurrentPattern(ValuePattern.Pattern, out var pattern))
            {
                return string.Empty;
            }

            var valuePattern = pattern as ValuePattern;
            if (valuePattern == null)
            {
                return string.Empty;
            }

            var parent = GetParentElement(element);
            if (parent == null)
            {
                return string.Empty;
            }

            var parentValue = GetValue(parent);
            if (parentValue != null)
            {
                return string.Empty;
            }

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                valuePattern.Current.Value);
            if (!FileUtil.IsDirectory(path))
            {
                return string.Empty;
            }

            return path;
        }

        private static AutomationElement? GetParentElement(AutomationElement element)
        {
            var walker = TreeWalker.ControlViewWalker;
            var parent = walker.GetParent(element);
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return null;
            }
        }

        private static AutomationElement? GetValue(AutomationElement element)
        {
            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out var pattern))
            {
                var valuePattern = pattern as AutomationElement;
                return valuePattern;
            }
            else
            {
                return null;
            }
        }
    }
}
