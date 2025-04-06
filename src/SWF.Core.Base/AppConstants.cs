using System.Runtime.InteropServices;
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
        public const int THUMBNAIL_MAXIMUM_SIZE = 256;
        public const int THUMBNAIL_MINIMUM_SIZE = 96;

        //private static readonly Lazy<bool> IS_RUNNING_AS_UWP
        //    = new(IsRunningAsUwp, LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> APPLICATION_DIRECTORY
            = new(GetApplicationDirectory, LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> LOG_DIRECTORY
            = new(() => Path.Combine(APPLICATION_DIRECTORY.Value, "log"), LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly Lazy<string> CONFIG_DIRECTORY
            = new(() => Path.Combine(APPLICATION_DIRECTORY.Value, "config"), LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> CONFIG_FILE
            = new(() => Path.Combine(CONFIG_DIRECTORY.Value, "config.dat"), LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> DATABASE_DIRECTORY
            = new(() => Path.Combine(APPLICATION_DIRECTORY.Value, "db"), LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> FILE_INFO_DATABASE_FILE
            = new(() => Path.Combine(DATABASE_DIRECTORY.Value, "fileinfo.sqlite"), LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> THUMBNAIL_DATABASE_FILE
            = new(() => Path.Combine(DATABASE_DIRECTORY.Value, "thumbnail.sqlite"), LazyThreadSafetyMode.ExecutionAndPublication);

        //private static bool IsRunningAsUwp()
        //{
        //    ConsoleUtil.Write(true, $"AppConstants.IsRunningAsUwp Start");
        //    try
        //    {
        //        // UWP の場合は Package.Current.Id が利用可能
        //        return Windows.ApplicationModel.Package.Current.Id != null;
        //    }
        //    catch
        //    {
        //        // 例外が発生した場合は UWP ではない
        //        return false;
        //    }
        //    finally
        //    {
        //        ConsoleUtil.Write(true, $"AppConstants.IsRunningAsUwp End");
        //    }
        //}

        private static string GetApplicationDirectory()
        {
#if UWP
            return Path.Combine(
                Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                "picsum.files");
#else
            var appFile = Environment.ProcessPath;
            if (string.IsNullOrEmpty(appFile))
            {
                throw new NullReferenceException("実行ファイルパスが取得できません。");
            }

            var appDir = Path.GetDirectoryName(appFile);
            if (string.IsNullOrEmpty(appDir))
            {
                throw new NullReferenceException("実行ディレクトリが取得できません。");
            }

            return appDir;
#endif
        }

        public static void CreateApplicationDirectories()
        {
            ConsoleUtil.Write(true, $"AppConstants.CreateApplicationDirectories Start");

            if (!FileUtil.IsExistsFileOrDirectory(APPLICATION_DIRECTORY.Value))
            {
                Directory.CreateDirectory(APPLICATION_DIRECTORY.Value);
            }

            if (!FileUtil.IsExistsFileOrDirectory(LOG_DIRECTORY.Value))
            {
                Directory.CreateDirectory(LOG_DIRECTORY.Value);
            }

            if (!FileUtil.IsExistsFileOrDirectory(CONFIG_DIRECTORY.Value))
            {
                Directory.CreateDirectory(CONFIG_DIRECTORY.Value);
            }

            if (!FileUtil.IsExistsFileOrDirectory(DATABASE_DIRECTORY.Value))
            {
                Directory.CreateDirectory(DATABASE_DIRECTORY.Value);
            }

            ConsoleUtil.Write(true, $"AppConstants.CreateApplicationDirectories End");
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

        public static Size GetControlBoxSize(IntPtr window)
        {
            if (WinApiMembers.DwmGetWindowAttribute(
                window,
                WinApiMembers.DWMWA_CAPTION_BUTTON_BOUNDS,
                out WinApiMembers.RECT rect,
                Marshal.SizeOf<WinApiMembers.RECT>()) == 0)
            {
                return new Size(
                    rect.right - rect.left,
                    rect.bottom - rect.top);
            }
            return Size.Empty;
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
