using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.Core.StringAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class FileListUtil
    {
        private static string[] GetSortFiles(
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
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FileName, NaturalStringComparer.WINDOWS)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderByDescending(file => file.FileName, NaturalStringComparer.WINDOWS)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                case SortTypeID.FilePath:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderByDescending(file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                case SortTypeID.CreateDate:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderBy(file => file.CreateDate)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderByDescending(file => file.CreateDate)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                case SortTypeID.UpdateDate:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderBy(file => file.UpdateDate)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderByDescending(file => file.UpdateDate)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                case SortTypeID.RegistrationDate:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderBy(file => file.RgistrationDate)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderByDescending(file => file.RgistrationDate)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                default:
                    return files
                        .AsValueEnumerable()
                        .Where(file => file.IsImageFile)
                        .Select(file => file.FilePath)
                        .ToArray();
            }
        }

        public static Action<ISender> ImageFilesGetActionForDirectory(ImageViewPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                var jobParameter = new FilesGetByDirectoryParameter()
                {
                    DirectoryPath = param.SourcesKey,
                    IsGetThumbnail = false,
                };

                Instance<JobCaller>.Value.EnqueueFilesGetByDirectoryJob(sender, jobParameter, e =>
                    {
                        var sortImageFiles = GetSortFiles(e.FileInfoList, param.SortInfo);
                        var eventArgs = new GetImageFilesEventArgs(
                            sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                        param.OnGetImageFiles(eventArgs);
                    });
            };
        }

        public static Action<ISender> ImageFilesGetActionForBookmark(ImageViewPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                var jobParameter = new FilesGetByDirectoryParameter()
                {
                    DirectoryPath = FileUtil.GetParentDirectoryPath(param.SelectedFilePath),
                    IsGetThumbnail = false,
                };

                Instance<JobCaller>.Value.EnqueueFilesGetByDirectoryJob(sender, jobParameter, e =>
                    {
                        var title = FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(param.SelectedFilePath));

                        var imageFiles = e.FileInfoList
                            .Where(fileInfo => fileInfo.IsImageFile)
                            .OrderBy(fileInfo => fileInfo.FilePath, NaturalStringComparer.WINDOWS)
                            .Select(fileInfo => fileInfo.FilePath)
                            .ToArray();

                        var eventArgs = new GetImageFilesEventArgs(
                            imageFiles, param.SelectedFilePath, title, Instance<IFileIconCacher>.Value.SmallDirectoryIcon);
                        param.OnGetImageFiles(eventArgs);
                    });
            };
        }

        public static Action<ISender> ImageFilesGetActionForRating(ImageViewPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                var jobParameter = new FilesGetByRatingParameter()
                {
                    RatingValue = int.Parse(param.SourcesKey),
                    IsGetThumbnail = false,
                };

                Instance<JobCaller>.Value.EnqueueFilesGetByRatingJob(sender, jobParameter, e =>
                    {
                        var sortImageFiles = GetSortFiles(e, param.SortInfo);
                        var eventArgs = new GetImageFilesEventArgs(
                            sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                        param.OnGetImageFiles(eventArgs);
                    });
            };
        }

        public static Action<ISender> ImageFilesGetActionForTag(ImageViewPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                var jobParameter = new FilesGetByTagParameter()
                {
                    Tag = param.SourcesKey,
                    IsGetThumbnail = false,
                };

                Instance<JobCaller>.Value.EnqueueFilesGetByTagJob(sender, jobParameter, e =>
                    {
                        var sortImageFiles = GetSortFiles(e, param.SortInfo);
                        var eventArgs = new GetImageFilesEventArgs(
                            sortImageFiles,
                            param.SelectedFilePath,
                            param.PageTitle,
                            param.PageIcon);
                        param.OnGetImageFiles(eventArgs);
                    });
            };
        }
    }
}
