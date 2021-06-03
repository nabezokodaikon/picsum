using PicSum.Core.Base.Conf;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// フォルダ状態
    /// </summary>
    public class FolderStateEntity : IEntity
    {
        public string FolderPath { get; set; }
        public SortTypeID SortTypeID { get; set; }
        public bool IsAscending { get; set; }
        public string SelectedFilePath { get; set; }
    }
}
