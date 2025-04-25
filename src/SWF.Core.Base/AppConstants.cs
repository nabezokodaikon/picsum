using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class AppConstants
    {
        public const string UI_THREAD_NAME = "Main";

        public const string MUTEX_NAME = "11d9bca9";
        public const string PIPE_NAME = "be752c43";

        private static Stopwatch? bootTimeStopwatch = null;

        public static void StartBootTimeMeasurement()
        {
            bootTimeStopwatch = Stopwatch.StartNew();
        }

        public static void StopBootTimeMeasurement()
        {
            bootTimeStopwatch?.Stop();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] Boot time: {bootTimeStopwatch?.ElapsedMilliseconds} ms");
        }

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
