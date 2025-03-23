using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    public struct DirectoryGetResult
        : IJobResult, IEquatable<DirectoryGetResult>
    {
        public string? DirectoryPath { get; internal set; }
        public DirectoryStateParameter DirectoryState { get; internal set; }
        public FileShallowInfoEntity[]? FileInfoList { get; internal set; }

        public bool Equals(DirectoryGetResult other)
        {
            if (this.DirectoryPath != other.DirectoryPath) { return false; }
            if (this.DirectoryState != other.DirectoryState) { return false; }
            if (this.FileInfoList != other.FileInfoList) { return false; }

            return true;
        }
    }
}
