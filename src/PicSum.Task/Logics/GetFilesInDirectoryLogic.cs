using PicSum.Core.Task.AsyncTask;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// フォルダ内のファイル取得非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class GetFilesInDirectoryLogic : AbstractAsyncLogic
    {
        public GetFilesInDirectoryLogic(AbstractAsyncTask task)
            : base(task)
        {

        }

        public IList<string> Execute(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("フォルダが指定されていません。", nameof(directoryPath));
            }

            var list = new List<string>();
            foreach (var file in FileUtil.GetFiles(directoryPath))
            {
                this.CheckCancel();
                if (FileUtil.CanAccess(file))
                {
                    list.Add(file);
                }
            }

            return list;
        }
    }
}
