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
using ZLinq;

namespace PicSum.UIComponent.Contents.FileList
{

    internal static class FileListUtil
    {
        private static string[] GetSortFiles(
            IEnumerable<FileShallowInfoEntity> files, SortParameter sortInfo)
        {
            ArgumentNullException.ThrowIfNull(files, nameof(files));
            ArgumentNullException.ThrowIfNull(sortInfo, nameof(sortInfo));

            var isAscending = sortInfo.IsAscending(sortInfo.ActiveSortMode);
            switch (sortInfo.ActiveSortMode)
            {
                case FileSortMode.FileName:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderBy(static file => file.FileName, NaturalStringComparer.WINDOWS)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderByDescending(static file => file.FileName, NaturalStringComparer.WINDOWS)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                case FileSortMode.FilePath:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderBy(static file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderByDescending(static file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                case FileSortMode.CreateDate:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderBy(static file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderBy(static file => file.CreateDate)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderBy(static file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderByDescending(static file => file.CreateDate)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                case FileSortMode.UpdateDate:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderBy(static file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderBy(static file => file.UpdateDate)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderBy(static file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderByDescending(static file => file.UpdateDate)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                case FileSortMode.TakenDate:
                    if (isAscending)
                    {
                        var a = files
                            .AsEnumerable()
                            .Where(static item => !item.TakenDate.IsEmpty())
                            .OrderBy(static item => item.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderBy(static item => item.TakenDate)
                            .Select(static item => item.FilePath)
                            .ToArray();
                        var b = files
                            .AsEnumerable()
                            .Where(static item => item.TakenDate.IsEmpty())
                            .OrderBy(static item => item.FilePath, NaturalStringComparer.WINDOWS)
                            .Select(static item => item.FilePath)
                            .ToArray();
                        return [.. a.Concat(b)];
                    }
                    else
                    {
                        var a = files
                            .AsEnumerable()
                            .Where(static item => !item.TakenDate.IsEmpty())
                            .OrderBy(static item => item.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderByDescending(static item => item.TakenDate)
                            .Select(static item => item.FilePath)
                            .ToArray();
                        var b = files
                            .AsEnumerable()
                            .Where(static item => item.TakenDate.IsEmpty())
                            .OrderBy(static item => item.FilePath, NaturalStringComparer.WINDOWS)
                            .Select(static item => item.FilePath)
                            .ToArray();
                        return [.. a.Concat(b)];
                    }
                case FileSortMode.AddDate:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderBy(static file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderBy(static file => file.RgistrationDate)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(static file => file.IsImageFile)
                            .OrderBy(static file => file.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderByDescending(static file => file.RgistrationDate)
                            .Select(static file => file.FilePath)
                            .ToArray();
                    }
                default:
                    return files
                        .AsValueEnumerable()
                        .Where(static file => file.IsImageFile)
                        .Select(static file => file.FilePath)
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
                            .AsEnumerable()
                            .Where(static fileInfo => fileInfo.IsImageFile)
                            .OrderBy(static fileInfo => fileInfo.FilePath, NaturalStringComparer.WINDOWS)
                            .Select(static fileInfo => fileInfo.FilePath)
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
