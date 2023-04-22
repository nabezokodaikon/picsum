using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PicSum.UIComponent.Common
{
    /// <summary>
    /// ファイル実行イベント引数クラス
    /// </summary>
    public class ExecuteFileEventArgs : EventArgs
    {
        private string _filePath = string.Empty;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public ExecuteFileEventArgs(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            _filePath = filePath;
        }
    }
}
