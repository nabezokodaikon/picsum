using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Paramters
{
    public sealed class FavoriteDirectoriesGetParameter
        : IJobParameter
    {
        public bool IsOnlyDirectory { get; set; }
        public int Count { get; set; }
    }
}
