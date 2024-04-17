using PicSum.Core.Job.AsyncJob;
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
        public string? DirectoryPath { get; set; }
        public DirectoryStateParameter? DirectoryState { get; set; }
        public IList<FileShallowInfoEntity>? FileInfoList { get; set; }
    }
}
