﻿using System;
using System.Collections.Generic;
using System.IO;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using SWF.Common;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// フォルダ内の画像ファイルを検索します。
    /// </summary>
    public class SearchImageFileByFolderAsyncFacade
        : TwoWayFacadeBase<SearchImageFileParameterEntity, SearchImageFileResultEntity>
    {
        public override void Execute(SearchImageFileParameterEntity param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            if (string.IsNullOrEmpty(param.FilePath))
            {
                throw new ArgumentException("空文字は無効です。", "param");
            }

            string folderPath;
            if (FileUtil.IsFolder(param.FilePath))
            {
                folderPath = param.FilePath;
            }
            else if (FileUtil.IsFile(param.FilePath))
            {
                folderPath = FileUtil.GetParentFolderPath(param.FilePath);
            }
            else
            {
                throw new ArgumentException("ファイルまたはフォルダのパスではありません。", "param");
            }

            SearchImageFileResultEntity result = new SearchImageFileResultEntity();
            result.FileOpenType = param.FileOpenType;
            result.TabIndex = param.TabIndex;

            IList<string> filePathList = null;
            try
            {
                GetFilesAndSubFoldersAsyncLogic getFilesLogic = new GetFilesAndSubFoldersAsyncLogic(this);
                filePathList = getFilesLogic.Execute(folderPath);
                result.DirectoryNotFoundException = null;
            }
            catch (DirectoryNotFoundException ex)
            {
                result.DirectoryNotFoundException = ex;
                OnCallback(result);
                return;
            }

            result.FilePathList = new List<string>();
            IList<string> exList = ImageUtil.ImageFileExtensionList;
            foreach (string filePath in filePathList)
            {
                string ex = FileUtil.GetExtension(filePath);
                if (exList.Contains(ex))
                {
                    result.FilePathList.Add(filePath);
                }
            }

            if (result.FilePathList.Contains(param.FilePath))
            {
                result.SelectedFilePath = param.FilePath;
            }
            else
            {
                result.SelectedFilePath = string.Empty;
            }

            OnCallback(result);
        }
    }
}
