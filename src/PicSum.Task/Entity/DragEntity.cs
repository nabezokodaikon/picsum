using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    public class DragEntity : IEntity
    {
        public string CurrentFilePath { get; set; }
        public IList<string> FilePathList { get; set; }
        public Control SourceControl { get; set; }
        public string ContentsTitle { get; set; }
        public Image ContentsIcon { get; set; }

        public DragEntity()
        {
            this.CurrentFilePath = string.Empty;
            this.FilePathList = null;
            this.SourceControl = null;
            this.ContentsTitle = string.Empty;
            this.ContentsIcon = null;
        }
    }
}
