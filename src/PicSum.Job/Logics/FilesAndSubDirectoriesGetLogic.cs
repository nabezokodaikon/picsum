using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ内のファイルとサブフォルダ取得非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FilesAndSubDirectoriesGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public string[] Execute(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var list = new List<string>();
            foreach (var file in FileUtil.GetFileSystemEntries(directoryPath))
            {
                this.CheckCancel();
                if (FileUtil.CanAccess(file))
                {
                    list.Add(file);
                }
            }

            return [.. list];
        }
    }
}
