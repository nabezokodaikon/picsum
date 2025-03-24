using PicSum.Job.Entities;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Results
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileDeepInfoGetResult
        : IJobResult
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
        public FileDeepInfoEntity FileInfo { get; internal set; } = FileDeepInfoEntity.EMPTY;
        public ListEntity<FileTagInfoEntity>? TagInfoList { get; internal set; }
        public bool IsError { get; private set; }
    }
}
