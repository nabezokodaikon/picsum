namespace SWF.Core.Base
{
    /// <summary>
    /// 例外ユーティリティクラス。
    /// </summary>

    public static class ExceptionUtil
    {
        public static void ShowErrorDialog(string message, Exception ex)
        {
#if DEBUG
            ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
            ArgumentNullException.ThrowIfNull(ex, nameof(ex));

            MessageBox.Show($"{message}\n{ex.Message}", "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
        }

        public static void ShowFatalDialog(string message, Exception ex)
        {
            ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
            ArgumentNullException.ThrowIfNull(ex, nameof(ex));

            MessageBox.Show($"{message}\n{ex.Message}", "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
