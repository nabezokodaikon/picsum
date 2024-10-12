using System.Runtime.Versioning;

namespace SWF.Core.Base
{
    /// <summary>
    /// 例外ユーティリティクラス。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class ExceptionUtil
    {
        public static void ShowErrorDialog(string message, Exception ex)
        {
            ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
            ArgumentNullException.ThrowIfNull(ex, nameof(ex));

            MessageBox.Show(message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
