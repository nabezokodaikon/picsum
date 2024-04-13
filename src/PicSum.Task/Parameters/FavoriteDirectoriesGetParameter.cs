using PicSum.Core.Task.AsyncTaskV2;

namespace PicSum.Task.Paramters
{
    public sealed class FavoriteDirectoriesGetParameter
        : ITaskParameter
    {
        public bool IsOnlyDirectory { get; set; }
        public int Count { get; set; }
    }
}
