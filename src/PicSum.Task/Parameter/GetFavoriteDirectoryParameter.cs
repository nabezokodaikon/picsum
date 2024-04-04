using PicSum.Core.Task.Base;

namespace PicSum.Task.Paramter
{
    public sealed class GetFavoriteDirectoryParameter
        : IEntity
    {
        public bool IsOnlyDirectory { get; set; }
        public int Count { get; set; }
    }
}
