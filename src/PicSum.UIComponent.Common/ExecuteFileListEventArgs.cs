using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PicSum.UIComponent.Common
{
    /// <summary>
    /// ファイルリスト実行イベント引数クラス
    /// </summary>
    public class ExecuteFileListEventArgs : EventArgs
    {
        private IList<string> _filePathList = null;

        public IList<string> FilePathList
        {
            get
            {
                return _filePathList;
            }
        }

        public ExecuteFileListEventArgs(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            _filePathList = filePathList;
        }
    }
}
