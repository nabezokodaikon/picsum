using System;
using System.Collections.Generic;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// フォルダ内のファイルとサブフォルダ取得非同期ロジック
    /// </summary>
    internal class GetFilesAndSubFoldersAsyncLogic : AsyncLogicBase
    {
        public GetFilesAndSubFoldersAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<string> Execute(string folderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentException("フォルダが指定されていません。", "folderPath");
            }

            List<string> list = new List<string>();
            foreach (string file in FileUtil.GetFilesAndSubFolders(folderPath))
            {
                CheckCancel();
                if (FileUtil.CanAccess(file))
                {                    
                    list.Add(file);
                }
            }

            return list;
        }
    }
}
