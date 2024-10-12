using PicSum.Core.Job.AsyncJob;
using System.Drawing;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// ファイルの深い情報取得パラメータエンティティ
    /// </summary>
    public struct FileDeepInfoGetParameter
        : IJobParameter
    {
        public IList<string>? FilePathList { get; set; }
        public Size ThumbnailSize { get; set; }
    }
}
