using System;
using System.Reflection;

namespace PicSum.Main
{
    public static class AppInfo
    {
        public static readonly Version CURRENT_VERSION
            = Assembly.GetExecutingAssembly().GetName().Version;

        public static bool IsUpdated(
            int majorVersion,
            int minorVersion,
            int buildVersion,
            int revisionVersion)
        {
            if (majorVersion < CURRENT_VERSION.Major)
            {
                return true;
            }
            else if (minorVersion < CURRENT_VERSION.Minor)
            {
                return true;
            }
            else if (buildVersion < CURRENT_VERSION.Build)
            {
                return true;
            }
            else if (revisionVersion < CURRENT_VERSION.Revision)
            {
                return true;
            }

            return false;
        }
    }
}
