﻿using PicSum.Core.Base.Conf;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Paramter
{
    public class GetImageFileByDirectoryParameter : IEntity
    {
        public string FilePath { get; set; }
        public ContentsOpenType FileOpenType { get; set; }
        public int TabIndex { get; set; }
    }
}