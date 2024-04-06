using PicSum.Core.Task.AsyncTaskV2;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// フォルダ内のファイルとサブフォルダ取得非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class GetFilesAndSubFoldersLogic
        : AbstractAsyncLogic
    {
        public GetFilesAndSubFoldersLogic(IAsyncTask task)
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
                throw new ArgumentException("フォルダが指定されていません。", "directoryPath");
            }

            var list = new List<string>();
            foreach (var file in FileUtil.GetFilesAndSubDirectorys(directoryPath))
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
