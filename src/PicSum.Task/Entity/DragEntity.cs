using System.Collections.Generic;
using System.Windows.Forms;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    public class DragEntity : IEntity
    {
        public string CurrentFilePath = string.Empty;
        public IList<string> FilePathList = null;
        public Control SourceControl = null;
    }
}
