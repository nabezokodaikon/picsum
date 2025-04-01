using System.Runtime.Versioning;
using WinApi;

namespace SWF.Core.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class AppConstants
    {
        public const string UI_THREAD_NAME = "Main";

        /// <summary>
        /// 番号付SQLパラメータの書式
        /// </summary>
        public const string NUMBERING_SQL_PARAMETER_FORMAT = "{0}_{1}";

        public const float BASE_DPI = 96f;

        /// <summary>
        /// 情報パネルの幅
        /// </summary>
        public const int INFOPANEL_WIDTH = 240;

        public const string MUTEX_NAME = "11d9bca9";
        public const string PIPE_NAME = "be752c43";

        public const string THUMBNAIL_BUFFER_FILE_EXTENSION = ".thumbnail";

        public static readonly string APPLICATION_DIRECTORY = GetApplicationDirectory();
        public static readonly string LOG_DIRECTORY = Path.Combine(APPLICATION_DIRECTORY, "log");
        private static readonly string CONFIG_DIRECTORY = Path.Combine(APPLICATION_DIRECTORY, "config");
        public static readonly string CONFIG_FILE = Path.Combine(CONFIG_DIRECTORY, "config.xml");
        public static readonly string DATABASE_DIRECTORY = Path.Combine(APPLICATION_DIRECTORY, "db");
        public static readonly string FILE_INFO_DATABASE_FILE = Path.Combine(DATABASE_DIRECTORY, "fileinfo.sqlite");
        public static readonly string THUMBNAIL_DATABASE_FILE = Path.Combine(DATABASE_DIRECTORY, "thumbnail.sqlite");

        public static bool IsRunningAsUwp()
        {
            ConsoleUtil.Write($"AppConstants.IsRunningAsUwp Start");
            try
            {
                // UWP の場合は Package.Current.Id が利用可能
                return Windows.ApplicationModel.Package.Current.Id != null;
            }
            catch
            {
                // 例外が発生した場合は UWP ではない
                return false;
            }
            finally
            {
                ConsoleUtil.Write($"AppConstants.IsRunningAsUwp End");
            }
        }

        public static float GetCursorWindowScale()
        {
            var hwnd = WinApiMembers.WindowFromPoint(
                new WinApiMembers.POINT(Cursor.Position.X, Cursor.Position.Y));
            var dpi = WinApiMembers.GetDpiForWindow(hwnd);
            var scale = dpi / AppConstants.BASE_DPI;
            return scale;
        }

        public static float GetCurrentWindowScale(Control control)
        {
            var dpi = WinApiMembers.GetDpiForWindow(control.Handle);
            var scale = dpi / AppConstants.BASE_DPI;
            return scale;
        }

        private static string GetExecutableDirectory()
        {
            ConsoleUtil.Write($"AppConstants.GetExecutableDirectory Start");
            try
            {
                var executableDirectory = Directory.GetParent(Application.ExecutablePath);
                if (executableDirectory == null)
                {
                    throw new NullReferenceException("実行ディレクトリが取得できません。");
                }

                return executableDirectory.FullName;
            }
            finally
            {
                ConsoleUtil.Write($"AppConstants.GetExecutableDirectory End");
            }
        }

        private static string GetApplicationDirectory()
        {
            if (AppConstants.IsRunningAsUwp())
            {
                return Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                    "picsum.files");
            }
            else
            {
                var executableDirectory = Directory.GetParent(Application.ExecutablePath);
                if (executableDirectory == null)
                {
                    throw new NullReferenceException("実行ディレクトリが取得できません。");
                }

                return executableDirectory.FullName;
            }
        }

        public static void CreateApplicationDirectories()
        {
            ConsoleUtil.Write($"AppConstants.CreateApplicationDirectories Start");

            if (!FileUtil.IsExistsFileOrDirectory(APPLICATION_DIRECTORY))
            {
                Directory.CreateDirectory(APPLICATION_DIRECTORY);
            }

            if (!FileUtil.IsExistsFileOrDirectory(LOG_DIRECTORY))
            {
                Directory.CreateDirectory(LOG_DIRECTORY);
            }

            if (!FileUtil.IsExistsFileOrDirectory(CONFIG_DIRECTORY))
            {
                Directory.CreateDirectory(CONFIG_DIRECTORY);
            }

            if (!FileUtil.IsExistsFileOrDirectory(DATABASE_DIRECTORY))
            {
                Directory.CreateDirectory(DATABASE_DIRECTORY);
            }

            ConsoleUtil.Write($"AppConstants.CreateApplicationDirectories End");
        }

        public static int GetControlBoxWidth(IntPtr window)
        {
            var monitor = WinApiMembers.MonitorFromWindow(window, WinApiMembers.MONITOR_DEFAULTTONEAREST);
            WinApiMembers.GetDpiForMonitor(monitor, WinApiMembers.MDT_EFFECTIVE_DPI, out uint dpiX, out _);
            var buttonWidth = WinApiMembers.GetSystemMetricsForDpi(WinApiMembers.SM.CXSIZE, dpiX);
            var frameWidth = WinApiMembers.GetSystemMetricsForDpi(WinApiMembers.SM.CXSIZEFRAME, dpiX);
            var paddingWidth = WinApiMembers.GetSystemMetricsForDpi(WinApiMembers.SM.CXPADDEDBORDER, dpiX);
            return (buttonWidth * 4) + paddingWidth + frameWidth;
        }

        public static int GetControlBoxHeight(IntPtr window)
        {
            var monitor = WinApiMembers.MonitorFromWindow(window, WinApiMembers.MONITOR_DEFAULTTONEAREST);
            WinApiMembers.GetDpiForMonitor(monitor, WinApiMembers.MDT_EFFECTIVE_DPI, out uint dpiX, out _);
            var titleBarHeight = WinApiMembers.GetSystemMetricsForDpi(WinApiMembers.SM.CYCAPTION, dpiX);
            var frameHeight = WinApiMembers.GetSystemMetricsForDpi(WinApiMembers.SM.CYSIZEFRAME, dpiX);
            var paddingWidth = WinApiMembers.GetSystemMetricsForDpi(WinApiMembers.SM.CXPADDEDBORDER, dpiX);
            return titleBarHeight + frameHeight + paddingWidth; ;
        }
    }

    /// <summary>
    /// コンテンツ表示種別
    /// </summary>
    public enum PageOpenType
    {
        Default = 0,
        OverlapTab = 1,
        AddHome = 2,
        AddTab = 3,
        InsertTab = 4,
        NewWindow = 5
    }

    /// <summary>
    /// 画像表示モード
    /// </summary>
    public enum ImageDisplayMode
    {
        Single = 0,
        LeftFacing = 1,
        RightFacing = 2
    }

    /// <summary>
    /// 画像サイズモード
    /// </summary>
    public enum ImageSizeMode
    {
        Original = 0,
        FitAllImage = 1,
        FitOnlyBigImage = 2
    }

    /// <summary>
    /// 画像表示位置
    /// </summary>
    public enum ImageAlign
    {
        Center = 0,
        Left = 1,
        LeftTop = 2,
        Top = 3,
        RightTop = 4,
        Right = 5,
        RightBottom = 6,
        Bottom = 7,
        LeftBottom = 8
    }

    /// <summary>
    /// ソート種別ID
    /// </summary>
    public enum SortTypeID
    {
        Default = 0,
        FileName = 1,
        FilePath = 2,
        UpdateDate = 3,
        RegistrationDate = 5,
    }
}
