using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace SWF.Common
{
    /// <summary>
    /// 例外ユーティリティクラス。
    /// </summary>
    public static class ExceptionUtil
    {
        /// <summary>
        /// 例外の詳細なメッセージを作成します。
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string CreateDetailsMessage(Exception ex) 
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            var sb = new StringBuilder();
            sb.AppendFormat("Message={0}\n", ex.Message);
            sb.AppendFormat("Source={0}\n", ex.Source);
            sb.AppendFormat("HelpLink={0}\n", ex.HelpLink);
            sb.AppendFormat("TargetSite={0}\n", ex.TargetSite.ToString());
            sb.AppendFormat("StackTrace={0}", ex.StackTrace);
            return sb.ToString();
        }

        public static void ShowErrorDialog(string message) 
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            MessageBox.Show(message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
