using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    public struct DirectoryGetResult
        : IJobResult
    {
        public string? DirectoryPath { get; internal set; }
        public DirectoryStateParameter DirectoryState { get; internal set; }
        public FileShallowInfoEntity[]? FileInfoList { get; internal set; }
    }
}
