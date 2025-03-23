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
            FilePathList = null,
            FileInfo = FileDeepInfoEntity.EMPTY,
            TagInfoList = null,
            IsError = false,
        };

        public static readonly FileDeepInfoGetResult ERROR = new()
        {
            FilePathList = null,
            FileInfo = FileDeepInfoEntity.ERROR,
            TagInfoList = null,
            IsError = true,
        };

        public string[]? FilePathList { get; internal set; }
        public FileDeepInfoEntity FileInfo { get; internal set; }
        public ListEntity<FileTagInfoEntity>? TagInfoList { get; internal set; }
        public bool IsError { get; private set; }

        public readonly bool Equals(FileDeepInfoGetResult other)
        {
            if (this.FilePathList != other.FilePathList) { return false; }
            if (this.FileInfo != other.FileInfo) { return false; }
            if (this.TagInfoList != other.TagInfoList) { return false; }
            if (this.IsError != other.IsError) { return false; }

            return true;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(FileDeepInfoGetResult))
            {
                return false;
            }

            return this.Equals((FileDeepInfoGetResult)obj);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.FilePathList, this.FileInfo, this.TagInfoList, this.IsError);
        }

        public static bool operator ==(FileDeepInfoGetResult left, FileDeepInfoGetResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FileDeepInfoGetResult left, FileDeepInfoGetResult right)
        {
            return !(left == right);
        }
    }
}
