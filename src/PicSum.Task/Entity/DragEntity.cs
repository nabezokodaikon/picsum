using PicSum.Core.Task.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.Task.Entity
{
    public class DragEntity : IEntity
    {
        public string CurrentFilePath { get; private set; }
        public IList<string> FilePathList { get; private set; }
        public Control SourceControl { get; private set; }
        public string ContentsTitle { get; private set; }
        public Image ContentsIcon { get; private set; }

        public DragEntity(string currentFilePath, IList<string> filePathList, Control sourceControl, string contentsTitle, Image contentsIcon)
        {
            this.CurrentFilePath = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));
            this.FilePathList = filePathList ?? throw new ArgumentNullException(nameof(filePathList));
            this.SourceControl = sourceControl ?? throw new ArgumentNullException(nameof(sourceControl));
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(currentFilePath));
        }

        public DragEntity(string currentFilePath, Control sourceControl, string contentsTitle, Image contentsIcon)
        {
            this.CurrentFilePath = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));
            this.FilePathList = new List<string>();
            this.SourceControl = sourceControl ?? throw new ArgumentNullException(nameof(sourceControl));
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(currentFilePath));
        }

        public DragEntity(string currentFilePath, string contentsTitle, Image contentsIcon)
        {
            this.CurrentFilePath = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));
            this.FilePathList = new List<string>();
            this.SourceControl = null;
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(currentFilePath));
        }
    }
}
