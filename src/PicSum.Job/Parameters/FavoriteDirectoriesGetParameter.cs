using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class FavoriteDirectoriesGetParameter
        : IJobParameter
    {
        public bool IsOnlyDirectory { get; set; }
        public int Count { get; set; }
    }
}
