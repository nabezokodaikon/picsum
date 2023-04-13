using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// サブフォルダ取得非同期ロジック
    /// </summary>
    public class GetSubDirectorysAsyncLogic : AsyncLogicBase
    {
        public GetSubDirectorysAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

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
            foreach (string subDirectory in FileUtil.GetSubDirectorys(directoryPath))
            {
                CheckCancel();
                if (FileUtil.CanAccess(subDirectory))
                {                    
                    list.Add(subDirectory);
                }
            }

            return list;
        }
    }
}
