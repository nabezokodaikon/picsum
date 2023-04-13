using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// フォルダ内のファイル取得非同期ロジック
    /// </summary>
    internal class GetFilesInDirectoryAsyncLogic:AsyncLogicBase
    {
        public GetFilesInDirectoryAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<string> Execute(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("フォルダが指定されていません。", "directoryPath");
            }

            List<string> list = new List<string>();
            foreach (string file in FileUtil.GetFiles(directoryPath))
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
