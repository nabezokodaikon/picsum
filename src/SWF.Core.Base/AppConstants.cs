namespace SWF.Core.Base
{
    public static class AppConstants
    {
        private const string UI_THREAD_NAME = "UI";

        public const string MUTEX_NAME = "fadae40f7f7e837033b2ac472808f03e7c9cce257210a11ff9cf4b8bf8b1ce0d";
        public const string PIPE_NAME = "2cb501081d1ec16e2feb1988139bcba70f0539d98b80d2d2fc83d0e167814331";
        public const float DEFAULT_ZOOM_VALUE = 1f;

        public static readonly int MAX_DEGREE_OF_PARALLELISM
            = Math.Min(16, Environment.ProcessorCount / 2);

        private static WindowsFormsSynchronizationContext? _uiContext = null;

        public static void SetUIThreadName()
        {
            if (Thread.CurrentThread.Name != null)
            {
                throw new InvalidOperationException("UIスレッド名が既に設定されています。");
            }

            Thread.CurrentThread.Name = UI_THREAD_NAME;
        }

        public static void ThrowIfNotUIThread()
        {
            if (Thread.CurrentThread.Name != UI_THREAD_NAME)
            {
                throw new InvalidOperationException("UIスレッドではありません。");
            }
        }

        public static SynchronizationContext GetUIThreadContext()
        {
            ThrowIfNotUIThread();

            if (_uiContext == null)
            {
                using (Measuring.Time(true, "AppConstants.GetUIThreadContext new WindowsFormsSynchronizationContext()"))
                {
                    _uiContext = new WindowsFormsSynchronizationContext();
                }
            }

            return _uiContext;
        }
    }

    /// <summary>
    /// コンテンツ表示モード
    /// </summary>
    public enum PageOpenMode
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
        FitOnlyBigImage = 2,
    }

    /// <summary>
    /// 画像表示位置
    /// </summary>
    public enum ImageAlign
    {
        Center = 0,
        Left = 1,
        Right = 2,
    }

    /// <summary>
    /// ソートモード
    /// </summary>
    public enum FileSortMode
    {
        Default = 0,
        FileName = 1,
        FilePath = 2,
        UpdateDate = 3,
        CreateDate = 4,
        AddDate = 5,
        TakenDate = 6,
    }
}
