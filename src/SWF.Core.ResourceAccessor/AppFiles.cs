using SWF.Core.ConsoleAccessor;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ResourceAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class AppFiles
    {
        //private static readonly Lazy<bool> IS_RUNNING_AS_UWP
        //    = new(IsRunningAsUwp);
        public static readonly Lazy<string> APPLICATION_DIRECTORY = new(
            GetApplicationDirectory,
            LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> LOG_DIRECTORY = new(
            () => Path.Combine(APPLICATION_DIRECTORY.Value, "log"),
            LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> CONFIG_DIRECTORY = new(
            () => Path.Combine(APPLICATION_DIRECTORY.Value, "config"),
            LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> CONFIG_FILE = new(
            () => Path.Combine(CONFIG_DIRECTORY.Value, "config.dat"),
            LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> DATABASE_DIRECTORY = new(
            () => Path.Combine(APPLICATION_DIRECTORY.Value, "db"),
            LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> FILE_INFO_DATABASE_FILE = new(
            () => Path.Combine(DATABASE_DIRECTORY.Value, "fileinfo.sqlite"),
            LazyThreadSafetyMode.ExecutionAndPublication);
        public static readonly Lazy<string> THUMBNAIL_DATABASE_FILE = new(
            () => Path.Combine(DATABASE_DIRECTORY.Value, "thumbnail.sqlite"),
            LazyThreadSafetyMode.ExecutionAndPublication);

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
            using (TimeMeasuring.Run(true, "AppFiles.CreateApplicationDirectories"))
            {
                if (!FileUtil.IsExistsDirectory(APPLICATION_DIRECTORY.Value))
                {
                    Directory.CreateDirectory(APPLICATION_DIRECTORY.Value);
                }

                if (!FileUtil.IsExistsDirectory(LOG_DIRECTORY.Value))
                {
                    Directory.CreateDirectory(LOG_DIRECTORY.Value);
                }

                if (!FileUtil.IsExistsDirectory(CONFIG_DIRECTORY.Value))
                {
                    Directory.CreateDirectory(CONFIG_DIRECTORY.Value);
                }

                if (!FileUtil.IsExistsDirectory(DATABASE_DIRECTORY.Value))
                {
                    Directory.CreateDirectory(DATABASE_DIRECTORY.Value);
                }
            }
        }

        //private static bool IsRunningAsUwp()
        //{
        //    ConsoleUtil.Write(true, $"AppFiles.IsRunningAsUwp Start");
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
        //        ConsoleUtil.Write(true, $"AppFiles.IsRunningAsUwp End");
        //    }
        //}
    }
}
