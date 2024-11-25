namespace SWF.Core.FileAccessor
{
    public static class ExplorerDragDrop
    {
        public static string GetExplorerPathAtCursor(int x, int y)
        {
            var desktopPath = ExplorerDesktopDragDrop.GetExplorerPathAtCursor(x, y);
            if (!string.IsNullOrEmpty(desktopPath))
            {
                return desktopPath;
            }

            var treePath = ExplorerTreeDragDrop.GetExplorerPathAtCursor(x, y);
            if (!string.IsNullOrEmpty(treePath))
            {
                return treePath;
            }

            var windowPath = ExplorerWindowDragDrop.GetExplorerPathAtCursor(x, y);
            if (!string.IsNullOrEmpty(windowPath))
            {
                return windowPath;
            }

            return string.Empty;
        }
    }
}
