using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// ファイルの評価値を更新するパラメータエンティティ
    /// </summary>
    public struct FileRatingUpdateParameter
        : IJobParameter
    {
        public IList<string>? FilePathList { get; set; }
        public int RatingValue { get; set; }
    }
}
