using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// ファイルの評価値を更新するパラメータエンティティ
    /// </summary>
    public struct FileRatingUpdateParameter
        : IJobParameter
    {
        public string[]? FilePathList { get; set; }
        public int RatingValue { get; set; }
    }
}
