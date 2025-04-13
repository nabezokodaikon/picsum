using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    public sealed class DirectoryGetResult
        : IJobResult
    {
        public string DirectoryPath { get; internal set; } = string.Empty;
        public DirectoryStateParameter DirectoryState { get; internal set; } = DirectoryStateParameter.EMPTY;
        public FileShallowInfoEntity[]? FileInfoList { get; internal set; }
    }
}
