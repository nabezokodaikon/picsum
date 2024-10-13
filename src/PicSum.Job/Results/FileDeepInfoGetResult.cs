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
        public IList<string>? FilePathList { get; internal set; }
        public FileDeepInfoEntity? FileInfo { get; internal set; }
        public ListEntity<FileTagInfoEntity>? TagInfoList { get; internal set; }
    }
}
