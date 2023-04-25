﻿using PicSum.Core.Task.Base;
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
        public Action GetImageFilesAction { get; private set; }
        public string ContentsTitle { get; private set; }
        public Image ContentsIcon { get; private set; }

        public DragEntity(
            string contentsSources, 
            string sourcesKey,
            string currentFilePath,
            Action getImageFilesAction, 
            string contentsTitle, 
            Image contentsIcon)
        {
            this.ContentsSources = contentsSources ?? throw new ArgumentNullException(nameof(contentsSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.CurrentFilePath = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));
            this.GetImageFilesAction = getImageFilesAction ?? throw new ArgumentNullException(nameof(getImageFilesAction));
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(contentsIcon));
        }

        public DragEntity(
            string contentsSources, 
            string sourcesKey, 
            string currentFilePath, 
            string contentsTitle, 
            Image contentsIcon)
        {
            this.ContentsSources = contentsSources ?? throw new ArgumentNullException(nameof(contentsSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.CurrentFilePath = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));
            this.GetImageFilesAction = null;
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(contentsIcon));
        }
    }
}
