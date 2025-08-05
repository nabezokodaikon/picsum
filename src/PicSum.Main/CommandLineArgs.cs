using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
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
            var arg = ARGS.FirstOrDefault(string.Empty).Trim();
            if (string.IsNullOrEmpty(arg))
            {
                return false;
            }

            if (arg != "--cleanup")
            {
                return false;
            }

            return true;
        }

        public static int GetThreshold()
        {
            var arg = ARGS.FirstOrDefault(string.Empty).Trim();
            if (string.IsNullOrEmpty(arg))
            {
                return -1;
            }

            if (!arg.StartsWith("--threshold="))
            {
                return -1;
            }

            var equalsIndex = arg.IndexOf('=');
            if (equalsIndex < 0)
            {
                return -1;
            }

            var numberString = arg.Substring(equalsIndex + 1);
            if (!int.TryParse(numberString, out int threshold))
            {
                return -1;
            }

            return threshold;
        }

        public static string GetImageFilePathCommandLineArgs()
        {
            var arg = ARGS.FirstOrDefault(string.Empty).Trim();
            if (string.IsNullOrEmpty(arg))
            {
                return string.Empty;
            }

            if (!FileUtil.CanAccess(arg) || !ImageUtil.IsImageFile(arg))
            {
                return string.Empty;
            }

            return arg;
        }
    }
}
