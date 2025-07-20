using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class FavoriteDirectoriesGetParameter
        : IJobParameter
    {
        public int Count { get; set; }
    }
}
