﻿using PicSum.Core.Task.Base;
using System.Collections.Generic;
using System.IO;

namespace PicSum.Task.Result
{
    public sealed class GetImageFileByDirectoryResult
        : IEntity
    {
        public string DirectoryPath { get; set; }
        public IList<string> FilePathList { get; set; }
        public string SelectedFilePath { get; set; }
        public DirectoryNotFoundException DirectoryNotFoundException { get; set; }
    }
}
