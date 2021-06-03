using System;
using System.Collections.Generic;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// サブフォルダ取得非同期ロジック
    /// </summary>
    public class GetSubFoldersAsyncLogic : AsyncLogicBase
    {
        public GetSubFoldersAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

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
            foreach (string subFolder in FileUtil.GetSubFolders(folderPath))
            {
                CheckCancel();
                if (FileUtil.CanAccess(subFolder))
                {                    
                    list.Add(subFolder);
                }
            }

            return list;
        }
    }
}
