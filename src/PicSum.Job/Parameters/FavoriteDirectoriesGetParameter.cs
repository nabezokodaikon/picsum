using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Parameters
{
    public struct FavoriteDirectoriesGetParameter
        : IJobParameter
    {
        public bool IsOnlyDirectory { get; set; }
        public int Count { get; set; }
    }
}
