using PicSum.Job.Entities;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>
    public struct FileDeepInfoGetResult
        : IJobResult
    {
        public static readonly FileDeepInfoGetResult EMPTY = new()
        {
            FilePathList = [],
            FileInfo = null,
            TagInfoList = [],
        };

        public string[]? FilePathList { get; internal set; }
        public FileDeepInfoEntity? FileInfo { get; internal set; }
        public ListEntity<FileTagInfoEntity>? TagInfoList { get; internal set; }
    }
}
