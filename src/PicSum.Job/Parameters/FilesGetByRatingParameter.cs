using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class FilesGetByRatingParameter
        : IJobParameter
    {
        public int RatingValue { get; set; }
    }
}
