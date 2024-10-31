namespace SWF.Core.Base
{
    public static class ResourceUtil
    {
        public const string THUMBNAIL_BUFFER_FILE_EXTENSION = ".thumbnail";

        public static readonly string EXECUTABLE_DIRECTORY = GetExecutableDirectory();
        public static readonly string APPLICATION_DIRECTORY = GetApplicationDirectory();
        public static readonly string LOG_DIRECTORY = GetLogDirectory();
        public static readonly string CONFIG_DIRECTORY = GetConfigDirectory();
        public static readonly string DATABASE_DIRECTORY = GetDatabaseDirectory();
        public static readonly string FILE_INFO_DATABASE_FILE = GetFileInfoDatabaseFile();
        public static readonly string THUMBNAIL_DATABASE_FILE = GetThumbnailDatabaseFile();
        public static readonly string RESOURCE_DIRECTORY = GetResourceDirectory();

        public static bool IsRunningAsUwp()
        {
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
        }

        private static string GetResourceDirectory()
        {
            return Path.Combine(EXECUTABLE_DIRECTORY, "ResourceFiles");
        }

        private static string GetFileInfoDatabaseFile()
        {
            return Path.Combine(DATABASE_DIRECTORY, "fileinfo.sqlite");
        }

        private static string GetThumbnailDatabaseFile()
        {
            return Path.Combine(DATABASE_DIRECTORY, "thumbnail.sqlite");
        }

        private static string GetExecutableDirectory()
        {
            var executableDirectory = Directory.GetParent(Application.ExecutablePath);
            if (executableDirectory == null)
            {
                throw new NullReferenceException("実行ディレクトリが取得できません。");
            }

            return executableDirectory.FullName;
        }

        private static string GetApplicationDirectory()
        {
            if (IsRunningAsUwp())
            {
                return Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                    "picsum.files");
            }
            else
            {
                return EXECUTABLE_DIRECTORY;
            }
        }

        private static string GetLogDirectory()
        {
            return Path.Combine(APPLICATION_DIRECTORY, "log");
        }

        private static string GetConfigDirectory()
        {
            return Path.Combine(APPLICATION_DIRECTORY, "config");
        }

        private static string GetDatabaseDirectory()
        {
            return Path.Combine(APPLICATION_DIRECTORY, "db");
        }

        public static void CreateApplicationDirectories()
        {
            if (!Directory.Exists(APPLICATION_DIRECTORY))
            {
                Directory.CreateDirectory(APPLICATION_DIRECTORY);
            }

            if (!Directory.Exists(LOG_DIRECTORY))
            {
                Directory.CreateDirectory(LOG_DIRECTORY);
            }

            if (!Directory.Exists(CONFIG_DIRECTORY))
            {
                Directory.CreateDirectory(CONFIG_DIRECTORY);
            }

            if (!Directory.Exists(DATABASE_DIRECTORY))
            {
                Directory.CreateDirectory(DATABASE_DIRECTORY);
            }
        }
    }
}
