using SWF.Core.Job;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ内のファイルとサブフォルダ取得非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FilessAndSubDirectoriesGetLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public IList<string> Execute(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

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
