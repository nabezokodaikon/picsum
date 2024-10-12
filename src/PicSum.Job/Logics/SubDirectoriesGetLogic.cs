using PicSum.Core.Job.AsyncJob;
using SWF.Core.FileAccessor;
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
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

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
