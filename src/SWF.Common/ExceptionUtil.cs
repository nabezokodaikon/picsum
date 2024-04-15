using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.Common
{
    /// <summary>
    /// 例外ユーティリティクラス。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class ExceptionUtil
    {
        public static void ShowErrorDialog(string message, Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            MessageBox.Show(message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
