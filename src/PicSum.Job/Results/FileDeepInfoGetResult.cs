using PicSum.Job.Entities;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>

    public sealed class FileDeepInfoGetResult
        : IJobResult
    {
        public static readonly FileDeepInfoGetResult EMPTY = new()
        {
            FilePathList = null,
            FileInfo = FileDeepInfoEntity.EMPTY,
            TagInfoList = null,
        };

        public static readonly FileDeepInfoGetResult ERROR = new()
        {
            FilePathList = null,
            FileInfo = FileDeepInfoEntity.ERROR,
            TagInfoList = null,
        };

        public bool IsEmpty
        {
            get
            {
                return this == EMPTY;
            }
        }

        public bool IsError
        {
            get
            {
                return this == ERROR;
            }
        }

        public string[]? FilePathList { get; internal set; }
        public FileDeepInfoEntity FileInfo { get; internal set; } = FileDeepInfoEntity.EMPTY;
        public ListEntity<FileTagInfoEntity>? TagInfoList { get; internal set; }

    }
}
