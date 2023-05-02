using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// デフォルトのエンティティ
    /// </summary>
    public sealed class DefaultEntity
        : IEntity
    {
        public object Args { get; set; }
    }
}
