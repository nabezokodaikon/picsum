using PicSum.Core.Job.AsyncJob;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// サブフォルダ取得非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class SubDirectoriesGetLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
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
            foreach (var subDirectory in FileUtil.GetSubDirectorys(directoryPath))
            {
                this.CheckCancel();
                if (FileUtil.CanAccess(subDirectory))
                {
                    list.Add(subDirectory);
                }
            }

            return list;
        }
    }
}
