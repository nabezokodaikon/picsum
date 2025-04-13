using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System;
using System.Linq;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class FileListUtil
    {
        private static string[] GetSortFiles(
            ListResult<FileShallowInfoEntity> files, SortInfo sortInfo)
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
                            .OrderBy(file => file.FileName, NaturalStringComparer.Windows)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderByDescending(file => file.FileName, NaturalStringComparer.Windows)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                case SortTypeID.FilePath:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FilePath, NaturalStringComparer.Windows)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderByDescending(file => file.FilePath, NaturalStringComparer.Windows)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                case SortTypeID.UpdateDate:
                    if (isAscending)
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderBy(file => file.FilePath, NaturalStringComparer.Windows)
                            .OrderBy(file => file.UpdateDate)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderByDescending(file => file.FilePath, NaturalStringComparer.Windows)
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
                            .OrderBy(file => file.FilePath, NaturalStringComparer.Windows)
                            .OrderBy(file => file.RgistrationDate)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                    else
                    {
                        return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .OrderByDescending(file => file.FilePath, NaturalStringComparer.Windows)
                            .OrderByDescending(file => file.RgistrationDate)
                            .Select(file => file.FilePath)
                            .ToArray();
                    }
                default:
                    return files
                            .AsValueEnumerable()
                            .Where(file => file.IsImageFile)
                            .Select(file => file.FilePath)
                            .ToArray(); ;
            }
        }

        public static Action<ISender> ImageFilesGetActionForDirectory(ImageViewerPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                var jobParameter = new FilesGetByDirectoryParameter()
                {
                    DirectoryPath = param.SourcesKey,
                    IsGetThumbnail = false,
                };

                Instance<JobCaller>.Value.FilesGetByDirectoryJob.Value
                    .StartJob(sender, jobParameter, e =>
                    {
                        var sortImageFiles = GetSortFiles(e.FileInfoList, param.SortInfo);
                        var eventArgs = new GetImageFilesEventArgs(
                            sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                        param.OnGetImageFiles(eventArgs);
                    });
            };
        }

        public static Action<ISender> ImageFilesGetActionForBookmark(ImageViewerPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                var jobParameter = new FilesGetByDirectoryParameter()
                {
                    DirectoryPath = FileUtil.GetParentDirectoryPath(param.SelectedFilePath),
                    IsGetThumbnail = false,
                };

                Instance<JobCaller>.Value.FilesGetByDirectoryJob.Value
                    .StartJob(sender, jobParameter, e =>
                    {
                        var title = FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(param.SelectedFilePath));

                        var imageFiles = e.FileInfoList
                            .AsValueEnumerable()
                            .Where(fileInfo => fileInfo.IsImageFile)
                            .OrderBy(fileInfo => fileInfo.FilePath, NaturalStringComparer.Windows)
                            .Select(fileInfo => fileInfo.FilePath)
                            .ToArray();

                        var eventArgs = new GetImageFilesEventArgs(
                            imageFiles, param.SelectedFilePath, title, Instance<IFileIconCacher>.Value.SmallDirectoryIcon);
                        param.OnGetImageFiles(eventArgs);
                    });
            };
        }

        public static Action<ISender> ImageFilesGetActionForRating(ImageViewerPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                var jobParameter = new FilesGetByRatingParameter()
                {
                    RatingValue = int.Parse(param.SourcesKey),
                    IsGetThumbnail = false,
                };

                Instance<JobCaller>.Value.FilesGetByRatingJob.Value
                    .StartJob(sender, jobParameter, e =>
                    {
                        var sortImageFiles = GetSortFiles(e, param.SortInfo);
                        var eventArgs = new GetImageFilesEventArgs(
                            sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                        param.OnGetImageFiles(eventArgs);
                    });
            };
        }

        public static Action<ISender> ImageFilesGetActionForTag(ImageViewerPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            return sender =>
            {
                var jobParameter = new FilesGetByTagParameter()
                {
                    Tag = param.SourcesKey,
                    IsGetThumbnail = false,
                };

                Instance<JobCaller>.Value.FilesGetByTagJob.Value
                    .StartJob(sender, jobParameter, e =>
                    {
                        var sortImageFiles = GetSortFiles(e, param.SortInfo);
                        var eventArgs = new GetImageFilesEventArgs(
                            sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                        param.OnGetImageFiles(eventArgs);
                    });
            };
        }
    }
}
