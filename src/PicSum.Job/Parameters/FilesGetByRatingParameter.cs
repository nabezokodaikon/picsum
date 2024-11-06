using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public struct FilesGetByRatingParameter
        : IJobParameter
    {
        public int RatingValue { get; set; }
        public bool IsGetThumbnail { get; set; }
    }
}
