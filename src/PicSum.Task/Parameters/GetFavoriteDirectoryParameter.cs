using PicSum.Core.Task.AsyncTaskV2;

namespace PicSum.Task.Paramters
{
    public sealed class GetFavoriteDirectoryParameter
        : ITaskParameter
    {
        public bool IsOnlyDirectory { get; set; }
        public int Count { get; set; }
    }
}
