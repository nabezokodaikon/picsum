using PicSum.Core.Job.AsyncJob;
using System.Drawing;

namespace PicSum.Job.Paramters
{
    /// <summary>
    /// ファイルの深い情報取得パラメータエンティティ
    /// </summary>
    public sealed class FileDeepInfoGetParameter
        : IJobParameter
    {
        public IList<string>? FilePathList { get; set; }
        public Size ThumbnailSize { get; set; }
    }
}
