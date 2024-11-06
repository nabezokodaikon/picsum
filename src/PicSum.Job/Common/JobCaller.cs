using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Common
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class JobCaller(SynchronizationContext context)
        : IDisposable
    {
        private bool disposed = false;

        private readonly Lazy<IOneWayJob<BookmarkAddJob, ValueParameter<string>>> addBookmarkJob
            = new(() => new OneWayTask<BookmarkAddJob, ValueParameter<string>>(context));
        private readonly Lazy<IOneWayJob<SingleFileExportJob, SingleFileExportParameter>> singleFileExportJob
            = new(() => new OneWayTask<SingleFileExportJob, SingleFileExportParameter>(context));
        private readonly Lazy<IOneWayJob<DirectoryStateUpdateJob, DirectoryStateParameter>> directoryStateUpdateJob
            = new(() => new OneWayTask<DirectoryStateUpdateJob, DirectoryStateParameter>(context));
        private readonly Lazy<IOneWayJob<DirectoryViewHistoryAddJob, ValueParameter<string>>> directoryViewHistoryAddJob
            = new(() => new OneWayTask<DirectoryViewHistoryAddJob, ValueParameter<string>>(context));
        private readonly Lazy<IOneWayJob<BookmarkDeleteJob, ListParameter<string>>> bookmarkDeleteJob
            = new(() => new OneWayTask<BookmarkDeleteJob, ListParameter<string>>(context));
        private readonly Lazy<IOneWayJob<DirectoryViewCounterDeleteJob, ListParameter<string>>> directoryViewCounterDeleteJob
            = new(() => new OneWayTask<DirectoryViewCounterDeleteJob, ListParameter<string>>(context));
        private readonly Lazy<IOneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter>> fileRatingUpdateJob
            = new(() => new OneWayTask<FileRatingUpdateJob, FileRatingUpdateParameter>(context));
        private readonly Lazy<IOneWayJob<FileTagDeleteJob, FileTagUpdateParameter>> fileTagDeleteJob
            = new(() => new OneWayTask<FileTagDeleteJob, FileTagUpdateParameter>(context));
        private readonly Lazy<IOneWayJob<FileTagAddJob, FileTagUpdateParameter>> fileTagAddJob
            = new(() => new OneWayTask<FileTagAddJob, FileTagUpdateParameter>(context));
        private readonly Lazy<IOneWayJob<GCCollectRunJob>> gcCollectRunJob
            = new(() => new OneWayTask<GCCollectRunJob>(context));

        public readonly Lazy<ITwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileReadJob
            = new(() => new TwoWayThread<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>(context));
        public readonly Lazy<ITwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileLoadingJob
            = new(() => new TwoWayThread<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>(context));
        public readonly Lazy<IOneWayJob<ImageFileCacheJob, ListParameter<string>>> ImageFileCacheJob
            = new(() => new OneWayTask<ImageFileCacheJob, ListParameter<string>>(context));
        public readonly Lazy<ITwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>> ThumbnailsGetJob
            = new(() => new TwoWayThread<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>(context));
        public readonly Lazy<ITwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>> SubDirectoriesGetJob
            = new(() => new TwoWayTask<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>> DirectoryViewHistoryGetJob
            = new(() => new TwoWayTask<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>> AddressInfoGetJob
            = new(() => new TwoWayTask<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>(context));
        public readonly Lazy<ITwoWayJob<TagsGetJob, ListResult<string>>> TagsGetJob
            = new(() => new TwoWayTask<TagsGetJob, ListResult<string>>(context));
        public readonly Lazy<ITwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoGetJob
            = new(() => new TwoWayTask<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>(context));
        public readonly Lazy<ITwoWayJob<PipeServerJob, ValueResult<string>>> PipeServerJob
            = new(() => new TwoWayTask<PipeServerJob, ValueResult<string>>(context));

        public readonly Lazy<ITwoWayJob<FilesGetByDirectoryJob, FilesGetByDirectoryParameter, DirectoryGetResult>> FilesGetByDirectoryJob
            = new(() => new TwoWayTask<FilesGetByDirectoryJob, FilesGetByDirectoryParameter, DirectoryGetResult>(context));
        public readonly Lazy<ITwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>> FavoriteDirectoriesGetJob
            = new(() => new TwoWayTask<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>>> FilesGetByRatingJob
            = new(() => new TwoWayTask<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>> FilesGetByTagJob
            = new(() => new TwoWayTask<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>> ImageFilesGetByDirectoryJob
            = new(() => new TwoWayTask<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>(context));
        public readonly Lazy<ITwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter<string>, ValueResult<string>>> NextDirectoryGetJob
            = new(() => new TwoWayTask<NextDirectoryGetJob, NextDirectoryGetParameter<string>, ValueResult<string>>(context));
        public readonly Lazy<ITwoWayJob<MultiFilesExportJob, MultiFilesExportParameter, ValueResult<string>>> MultiFilesExportJob
            = new(() => new TwoWayTask<MultiFilesExportJob, MultiFilesExportParameter, ValueResult<string>>(context));
        public readonly Lazy<ITwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>> BookmarksGetJob
            = new(() => new TwoWayTask<BookmarksGetJob, ListResult<FileShallowInfoEntity>>(context));

        ~JobCaller()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.addBookmarkJob.Value.Dispose();
                this.singleFileExportJob.Value.Dispose();
                this.directoryStateUpdateJob.Value.Dispose();
                this.directoryViewHistoryAddJob.Value.Dispose();
                this.bookmarkDeleteJob.Value.Dispose();
                this.directoryViewCounterDeleteJob.Value.Dispose();
                this.fileRatingUpdateJob.Value.Dispose();
                this.fileTagDeleteJob.Value.Dispose();
                this.fileTagAddJob.Value.Dispose();
                this.gcCollectRunJob.Value.Dispose();

                this.ImageFileReadJob.Value.Dispose();
                this.ImageFileLoadingJob.Value.Dispose();
                this.ImageFileCacheJob.Value.Dispose();
                this.ThumbnailsGetJob.Value.Dispose();
                this.SubDirectoriesGetJob.Value.Dispose();
                this.DirectoryViewHistoryGetJob.Value.Dispose();
                this.AddressInfoGetJob.Value.Dispose();
                this.TagsGetJob.Value.Dispose();
                this.FileDeepInfoGetJob.Value.Dispose();
                this.PipeServerJob.Value.Dispose();

                this.FilesGetByDirectoryJob.Value.Dispose();
                this.FavoriteDirectoriesGetJob.Value.Dispose();
                this.FilesGetByRatingJob.Value.Dispose();
                this.FilesGetByTagJob.Value.Dispose();
                this.ImageFilesGetByDirectoryJob.Value.Dispose();
                this.NextDirectoryGetJob.Value.Dispose();
                this.MultiFilesExportJob.Value.Dispose();
                this.BookmarksGetJob.Value.Dispose();
            }

            this.disposed = true;
        }

        public void StartBookmarkAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.addBookmarkJob.Value.Reset()
                .StartJob(sender, parameter);
        }

        public void StartSingleFileExportJob(ISender sender, SingleFileExportParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.singleFileExportJob.Value.Reset()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryStateUpdateJob(ISender sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryStateUpdateJob.Value.Reset()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewHistoryAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryViewHistoryAddJob.Value.Reset()
                .StartJob(sender, parameter);
        }

        public void StartBookmarkDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.bookmarkDeleteJob.Value.Reset()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewCounterDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryViewCounterDeleteJob.Value.Reset()
                .StartJob(sender, parameter);
        }

        public void StartFileRatingUpdateJob(ISender sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileRatingUpdateJob.Value.Reset()
                .StartJob(sender, parameter);
        }

        public void StartFileTagDeleteJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileTagDeleteJob.Value.Reset()
                .StartJob(sender, parameter);
        }

        public void StartFileTagAddJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileTagAddJob.Value.Reset()
                .StartJob(sender, parameter);
        }

        public void StartGCCollectRunJob(ISender sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.gcCollectRunJob.Value.Reset()
                .StartJob(sender);
        }
    }
}
