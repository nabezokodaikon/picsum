using SWF.Core.FileAccessor;
using System;
using System.Linq;

namespace PicSum.Main
{
    internal static class CommandLineArgs
    {
        private static readonly string[] ARGS
            = [.. Environment.GetCommandLineArgs().Skip(1)];

        public static bool IsNone()
        {
            return ARGS.Length == 0;
        }

        public static bool IsCleanup()
        {
            return ARGS.Contains("--cleanup");
        }

        public static bool IsEmpty()
        {
            return ARGS.Contains("--empty");
        }

        public static string GetImageFilePathCommandLineArgs()
        {
            return ARGS
                .FirstOrDefault(_ => FileUtil.CanAccess(_) && FileUtil.IsImageFile(_));
        }
    }
}
