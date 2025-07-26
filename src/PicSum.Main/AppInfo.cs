using System;
using System.Reflection;

namespace PicSum.Main
{
    public static class AppInfo
    {
        public static readonly Version CURRENT_VERSION
            = Assembly.GetExecutingAssembly().GetName().Version;
    }
}
