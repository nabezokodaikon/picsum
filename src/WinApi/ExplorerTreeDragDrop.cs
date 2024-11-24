using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Automation;

namespace WinApi
{
    public partial class ExplorerTreeDragDrop
    {
        private static readonly Regex DriveLetterRegex = new Regex(@"\((\w:)\)");

        public static string GetExplorerPathAtCursor(int x, int y)
        {
            var target = AutomationElement.FromPoint(new System.Windows.Point(x, y));
            if (target == null)
            {
                return string.Empty;
            }

            var listView = TreeWalker.ControlViewWalker.GetParent(target);
            if (listView == null)
            {
                return string.Empty;
            }

            var directoryNameList = new List<string>
            {
                GetValue(target)
            };

            var temp = target;
            while (temp != null)
            {
                var parent = GetParentElement(temp);
                if (parent == null)
                {
                    break;
                }

                var value = GetValue(parent);
                if (!string.IsNullOrEmpty(value))
                {
                    directoryNameList.Insert(0, value);
                }

                temp = parent;
            }

            if (directoryNameList.Count < 1)
            {
                return string.Empty;
            }

            while (directoryNameList.Count > 0)
            {
                var item = directoryNameList.First();
                var match = DriveLetterRegex.Match(item);
                if (match.Success)
                {
                    var driveLetter = match.Groups[1].Value;
                    directoryNameList[0] = driveLetter;
                    break;
                }
                else
                {
                    directoryNameList.RemoveAt(0);
                }
            }

            var fullPath = string.Empty;
            foreach (var name in directoryNameList)
            {
                fullPath = Path.Combine(fullPath, name);
            }

            return fullPath;
        }

        private static string GetValue(AutomationElement element)
        {
            object pattern;
            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out pattern))
            {
                var valuePattern = (ValuePattern)pattern;
                return valuePattern.Current.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        private static AutomationElement GetParentElement(AutomationElement element)
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
    }
}
