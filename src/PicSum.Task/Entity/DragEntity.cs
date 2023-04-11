using PicSum.Core.Task.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.Task.Entity
{
    public class DragEntity : IEntity
    {
        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string CurrentFilePath { get; private set; }
        public IList<string> FilePathList { get; private set; }
        public string ContentsTitle { get; private set; }
        public Image ContentsIcon { get; private set; }

        public DragEntity(string contentsSources, string sourcesKey, string currentFilePath, IList<string> filePathList, string contentsTitle, Image contentsIcon)
        {
            this.ContentsSources = contentsSources ?? throw new ArgumentNullException(nameof(contentsSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.CurrentFilePath = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));
            this.FilePathList = filePathList ?? throw new ArgumentNullException(nameof(filePathList));
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(currentFilePath));
        }

        public DragEntity(string contentsSources, string sourcesKey, string currentFilePath, string contentsTitle, Image contentsIcon)
        {
            this.ContentsSources = contentsSources ?? throw new ArgumentNullException(nameof(contentsSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.CurrentFilePath = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));
            this.FilePathList = new List<string>();
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(currentFilePath));
        }
    }
}
