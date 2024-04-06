using PicSum.Core.Task.Base;

namespace PicSum.Task.Entities
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
