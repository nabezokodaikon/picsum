using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System;
using System.Linq;

namespace PicSum.Main
{
    internal static class CommandLineArgs
    {
        public static bool IsCleanup()
        {
            return Environment.GetCommandLineArgs().Contains("--cleanup");
        }

        public static bool IsHome()
        {
            if (AppConstants.IsRunningAsUwp())
            {
                return true;
            }
            else
            {
                return Environment.GetCommandLineArgs().Contains("--home");
            }
        }

        public static bool IsFilePath()
        {
            return Environment.GetCommandLineArgs()
                .Any(_ => FileUtil.CanAccess(_) && FileUtil.IsImageFile(_));
        }

        public static string GetImageFilePatCommandLineArgs()
        {
            return Environment.GetCommandLineArgs()
                .FirstOrDefault(_ => FileUtil.CanAccess(_) && FileUtil.IsImageFile(_));
        }
    }
}
