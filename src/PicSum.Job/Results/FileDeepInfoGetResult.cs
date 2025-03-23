using PicSum.Job.Entities;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Results
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public struct FileDeepInfoGetResult
        : IJobResult, IEquatable<FileDeepInfoGetResult>
    {
        public static readonly FileDeepInfoGetResult EMPTY = new()
        {
            FilePathList = [],
            FileInfo = FileDeepInfoEntity.EMPTY,
            TagInfoList = [],
        };

        public string[]? FilePathList { get; internal set; }
        public FileDeepInfoEntity FileInfo { get; internal set; }
        public ListEntity<FileTagInfoEntity>? TagInfoList { get; internal set; }

        public bool Equals(FileDeepInfoGetResult other)
        {
            if (this.FilePathList != other.FilePathList) { return false; }
            if (!this.FileInfo.Equals(other.FileInfo)) { return false; }
            if (this.TagInfoList != other.TagInfoList) { return false; }

            return true;
        }
    }
}
