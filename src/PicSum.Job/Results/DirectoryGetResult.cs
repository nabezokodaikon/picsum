using SWF.Core.Job;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;

namespace PicSum.Job.Results
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    public sealed class DirectoryGetResult
        : IJobResult
    {
        public string? DirectoryPath { get; internal set; }
        public DirectoryStateParameter DirectoryState { get; internal set; }
        public IList<FileShallowInfoEntity>? FileInfoList { get; internal set; }
    }
}
