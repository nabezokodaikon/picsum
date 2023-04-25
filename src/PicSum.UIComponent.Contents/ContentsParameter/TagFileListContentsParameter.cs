﻿using PicSum.UIComponent.Contents.FileListContents;
using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// タグファイルリストコンテンツパラメータ
    /// </summary>
    public sealed class TagFileListContentsParameter
        : IContentsParameter
    {
        public const string CONTENTS_SOURCES = "Tag";

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public String Tag { get; private set; }
        public string SelectedFilePath { get; set; }

        public TagFileListContentsParameter(string tag)
        {
            this.ContentsSources = TagFileListContentsParameter.CONTENTS_SOURCES;
            this.SourcesKey = tag;
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            this.Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            this.SelectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new TagFileListContents(this);
        }
    }
}
