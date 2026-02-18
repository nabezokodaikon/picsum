using System.Runtime.InteropServices;
using WinApi;

namespace SWF.Core.Base
{
    public static class DisplayUtil
    {
        public static int GetAnimationInterval(Control control)
        {
            ArgumentNullException.ThrowIfNull(control, nameof(control));

            using (Measuring.Time(false, "DisplayUitl.GetAnimationInterval"))
            {
                var displaySize = GetDisplaySize(control);
                var refreshRate = GetRefreshRate(control);
                ConsoleUtil.Write(false, $"Display size: {displaySize}, Refresh rate: {refreshRate}");

                if (displaySize.Width > 1920 && displaySize.Height > 1080)
                {
                    return Math.Max(1000 / 60, 1000 / refreshRate);
                }
                else
                {
                    return Math.Max(1000 / 90, 1000 / refreshRate);
                }
            }
        }

        private static Size GetDisplaySize(Control control)
        {
            var currentScreen = Screen.FromControl(control);
            return currentScreen.Bounds.Size;
        }

        private static int GetRefreshRate(Control control)
        {
            var screen = Screen.FromControl(control);

            var devMode = new WinApiMembers.DEVMODE();
            devMode.dmSize = (short)Marshal.SizeOf(devMode);

            WinApiMembers.EnumDisplaySettings(screen.DeviceName, WinApiMembers.ENUM_CURRENT_SETTINGS, ref devMode);
            return devMode.dmDisplayFrequency;
        }
    }
}
