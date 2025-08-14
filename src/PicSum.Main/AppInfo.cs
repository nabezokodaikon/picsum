using System;
using System.Reflection;

namespace PicSum.Main
{
    internal static class AppInfo
    {
        public static readonly Version CURRENT_VERSION
            = Assembly.GetExecutingAssembly().GetName().Version;
    }
}
