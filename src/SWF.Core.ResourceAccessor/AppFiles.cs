namespace SWF.Core.ResourceAccessor
{

    public static class AppFiles
    {
        public const string PROFILE_FILE_NAME = "startup.profile";

        public static readonly Lazy<string> APPLICATION_DIRECTORY
            = new(() =>
            {
                var dir = GetApplicationDirectory();
                Directory.CreateDirectory(dir);
                return dir;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<string> PROFILE_DIRECTORY
            = new(
            () =>
            {
                var dir = Path.Combine(APPLICATION_DIRECTORY.Value, "profile");
                Directory.CreateDirectory(dir);
                return dir;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<string> LOG_DIRECTORY
            = new(
            () =>
            {
                var dir = Path.Combine(APPLICATION_DIRECTORY.Value, "log");
                Directory.CreateDirectory(dir);
                return dir;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<string> CONFIG_DIRECTORY
            = new(
            () =>
            {
                var dir = Path.Combine(APPLICATION_DIRECTORY.Value, "config");
                Directory.CreateDirectory(dir);
                return dir;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<string> DATABASE_DIRECTORY
            = new(
            () =>
            {
                var dir = Path.Combine(APPLICATION_DIRECTORY.Value, "db");
                Directory.CreateDirectory(dir);
                return dir;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<string> CONFIG_FILE = new(
            static () => Path.Combine(CONFIG_DIRECTORY.Value, "config.dat"),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<string> FILE_INFO_DATABASE_FILE = new(
            static () => Path.Combine(DATABASE_DIRECTORY.Value, "fileinfo.sqlite"),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<string> THUMBNAIL_DATABASE_FILE = new(
            static () => Path.Combine(DATABASE_DIRECTORY.Value, "thumbnail.sqlite"),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<string> THUMBNAIL_CACHE_FILE = new(
            static () => Path.Combine(DATABASE_DIRECTORY.Value, "thumbnail.cache"),
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
                throw new InvalidOperationException("実行ファイルパスが取得できません。");
            }

            var appDir = Path.GetDirectoryName(appFile);
            if (string.IsNullOrEmpty(appDir))
            {
                throw new InvalidOperationException("実行ディレクトリが取得できません。");
            }

            return appDir;
#endif
        }
    }
}
