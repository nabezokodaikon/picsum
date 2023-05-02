using PicSum.Core.Task.Base;

namespace PicSum.Task.Paramter
{
    public sealed class GetFavoriteFolderParameter
        : IEntity
    {
        public bool IsOnlyDirectory { get; set; }
        public int Count { get; set; }
    }
}
