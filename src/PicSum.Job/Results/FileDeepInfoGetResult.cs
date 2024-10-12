using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;

namespace PicSum.Job.Results
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>
    public sealed class FileDeepInfoGetResult
        : IJobResult
    {
        public IList<string>? FilePathList;
        public FileDeepInfoEntity? FileInfo;
        public ListEntity<FileTagInfoEntity>? TagInfoList;
    }
}
