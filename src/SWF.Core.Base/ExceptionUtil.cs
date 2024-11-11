using System.Runtime.Versioning;

namespace SWF.Core.Base
{
    /// <summary>
    /// 例外ユーティリティクラス。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class ExceptionUtil
    {
        public static void ShowErrorDialog(string message, Exception ex)
        {
#if DEBUG
            ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
            ArgumentNullException.ThrowIfNull(ex, nameof(ex));

            MessageBox.Show(message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
#elif DEVELOP
            ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
            ArgumentNullException.ThrowIfNull(ex, nameof(ex));

            MessageBox.Show(message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
        }

        public static void ShowFatalDialog(string message, Exception ex)
        {
            ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
            ArgumentNullException.ThrowIfNull(ex, nameof(ex));

            MessageBox.Show(message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
