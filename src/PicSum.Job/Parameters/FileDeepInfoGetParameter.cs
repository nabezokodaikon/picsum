using SWF.Core.Job;
using System.Drawing;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// ファイルの深い情報取得パラメータエンティティ
    /// </summary>
    public sealed class FileDeepInfoGetParameter
        : IJobParameter
    {
        public string[]? FilePathList { get; set; }
        public Size ThumbnailSize { get; set; }
    }
}
