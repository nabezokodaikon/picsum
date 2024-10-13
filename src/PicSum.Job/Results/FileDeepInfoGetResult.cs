using SWF.Core.Job;
using PicSum.Job.Entities;

namespace PicSum.Job.Results
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>
    public sealed class FileDeepInfoGetResult
        : IJobResult
    {
        public IList<string>? FilePathList { get; internal set; }
        public FileDeepInfoEntity? FileInfo { get; internal set; }
        public ListEntity<FileTagInfoEntity>? TagInfoList { get; internal set; }
    }
}
