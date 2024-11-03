using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows")]
    internal static class FileListUtil
    {
        private static IEnumerable<FileShallowInfoEntity> GetSortFiles(
            IEnumerable<FileShallowInfoEntity> files, SortInfo sortInfo)
        {
            ArgumentNullException.ThrowIfNull(files, nameof(files));
            ArgumentNullException.ThrowIfNull(sortInfo, nameof(sortInfo));

            var isAscending = sortInfo.IsAscending(sortInfo.ActiveSortType);
            switch (sortInfo.ActiveSortType)
            {
                case SortTypeID.FileName:
                    if (isAscending)
                    {
                        return files.OrderBy(file => file.FileName, NaturalStringComparer.Windows);
                    }
                    else
                    {
                        return files.OrderByDescending(file => file.FileName, NaturalStringComparer.Windows);
                    }
                case SortTypeID.FilePath:
                    if (isAscending)
                    {
                        return files.OrderBy(file => file.FilePath, NaturalStringComparer.Windows);
                    }
                    else
                    {
                        return files.OrderByDescending(file => file.FilePath, NaturalStringComparer.Windows);
                    }
                case SortTypeID.UpdateDate:
                    if (isAscending)
                    {
                        return files
                            .OrderBy(file => file.FilePath, NaturalStringComparer.Windows)
                            .OrderBy(file => file.UpdateDate.GetValueOrDefault(DateTime.MinValue));
                    }
                    else
                    {
                        return files
                            .OrderByDescending(file => file.FilePath, NaturalStringComparer.Windows)
                            .OrderByDescending(file => file.UpdateDate.GetValueOrDefault(DateTime.MinValue));
                    }
                case SortTypeID.RegistrationDate:
                    if (isAscending)
                    {
                        return files
                            .OrderBy(file => file.FilePath, NaturalStringComparer.Windows)
                            .OrderBy(file => file.RgistrationDate.GetValueOrDefault(DateTime.MinValue));
                    }
                    else
                    {
                        return files
                            .OrderByDescending(file => file.FilePath, NaturalStringComparer.Windows)
                            .OrderByDescending(file => file.RgistrationDate.GetValueOrDefault(DateTime.MinValue));
                    }
                default:
                    return files;
            }
        }

        public static Action<ISender> ImageFilesGetActionForDirectory(ImageViewerPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                JobCaller.Instance.FilesGetByDirectoryJob.Value
                    .Initialize()
                    .Callback(e =>
                    {
                        var imageFiles = e.FileInfoList
                            .Where(fileInfo => fileInfo.IsImageFile);
                        var sortImageFiles = GetSortFiles(imageFiles, param.SortInfo)
                            .Select(fileInfo => fileInfo.FilePath)
                            .ToArray();

                        if (!FileUtil.IsImageFile(param.SelectedFilePath))
                        {
                            throw new SWFException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                        }

                        var eventArgs = new GetImageFilesEventArgs(
                            sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                        param.OnGetImageFiles(eventArgs);
                    })
                    .BeginCancel()
                    .StartJob(sender, new ValueParameter<string>(param.SourcesKey));
            };
        }

        public static Action<ISender> ImageFilesGetActionForBookmark(ImageViewerPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                var dir = FileUtil.GetParentDirectoryPath(param.SelectedFilePath);

                JobCaller.Instance.FilesGetByDirectoryJob.Value
                    .Initialize()
                    .Callback(e =>
                    {
                        if (!FileUtil.IsImageFile(param.SelectedFilePath))
                        {
                            throw new SWFException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                        }

                        var title = FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(param.SelectedFilePath));

                        var imageFiles = e.FileInfoList
                            .Where(fileInfo => fileInfo.IsImageFile)
                            .OrderBy(fileInfo => fileInfo.FilePath, NaturalStringComparer.Windows)
                            .Select(fileInfo => fileInfo.FilePath)
                            .ToArray();

                        var eventArgs = new GetImageFilesEventArgs(
                            imageFiles, param.SelectedFilePath, title, FileIconCacher.Instance.SmallDirectoryIcon);
                        param.OnGetImageFiles(eventArgs);
                    })
                    .BeginCancel()
                    .StartJob(sender, new ValueParameter<string>(dir));
            };
        }

        public static Action<ISender> ImageFilesGetActionForRating(ImageViewerPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                JobCaller.Instance.FilesGetByRatingJob.Value
                    .Initialize()
                    .Callback(e =>
                    {
                        var imageFiles = e
                            .Where(fileInfo => fileInfo.IsImageFile);
                        var sortImageFiles = GetSortFiles(imageFiles, param.SortInfo)
                            .Select(fileInfo => fileInfo.FilePath)
                            .ToArray();

                        if (!FileUtil.IsImageFile(param.SelectedFilePath))
                        {
                            throw new SWFException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                        }

                        var eventArgs = new GetImageFilesEventArgs(
                            sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                        param.OnGetImageFiles(eventArgs);
                    })
                    .BeginCancel()
                    .StartJob(sender, new ValueParameter<int>(int.Parse(param.SourcesKey)));
            };
        }

        public static Action<ISender> ImageFilesGetActionForTag(ImageViewerPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                JobCaller.Instance.FilesGetByTagJob.Value
                    .Initialize()
                    .Callback(e =>
                    {
                        var imageFiles = e
                            .Where(fileInfo => fileInfo.IsImageFile);
                        var sortImageFiles = GetSortFiles(imageFiles, param.SortInfo)
                            .Select(fileInfo => fileInfo.FilePath)
                            .ToArray();

                        if (!FileUtil.IsImageFile(param.SelectedFilePath))
                        {
                            throw new SWFException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                        }

                        var eventArgs = new GetImageFilesEventArgs(
                            sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                        param.OnGetImageFiles(eventArgs);
                    })
                    .BeginCancel()
                    .StartJob(sender, new ValueParameter<string>(param.SourcesKey));
            };
        }
    }
}
