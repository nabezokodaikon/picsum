using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Common
{
    [SupportedOSPlatform("windows")]
    public sealed partial class CommonJobs
        : IDisposable
    {
        private static SynchronizationContext? context = null;
        private static CommonJobs? instance = null;

        public static CommonJobs Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("CommonJobsのインスタンスが設定されていません。");
                }

                return instance;
            }
        }

        public static void Initialize()
        {
            if (context != null)
            {
                throw new InvalidOperationException("同期コンテキストは既に設定されています。");
            }

            if (SynchronizationContext.Current == null)
            {
                throw new InvalidOperationException("同期コンテキストが存在しません。");
            }

            context = SynchronizationContext.Current;
            instance = new CommonJobs();
        }

        private bool disposed = false;

        private readonly Lazy<OneWayJob<BookmarkAddJob, ValueParameter<string>>> addBookmarkJob = new(() => new(context));
        private readonly Lazy<OneWayJob<SingleFileExportJob, SingleFileExportParameter>> singleFileExportJob = new(() => new(context));
        private readonly Lazy<OneWayJob<DirectoryStateUpdateJob, DirectoryStateParameter>> directoryStateUpdateJob = new(() => new(context));
        private readonly Lazy<OneWayJob<DirectoryViewHistoryAddJob, ValueParameter<string>>> directoryViewHistoryAddJob = new(() => new(context));
        private readonly Lazy<OneWayJob<BookmarkDeleteJob, ListParameter<string>>> bookmarkDeleteJob = new(() => new(context));
        private readonly Lazy<OneWayJob<DirectoryViewCounterDeleteJob, ListParameter<string>>> directoryViewCounterDeleteJob = new(() => new(context));
        private readonly Lazy<OneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter>> fileRatingUpdateJob = new(() => new(context));
        private readonly Lazy<OneWayJob<FileTagDeleteJob, FileTagUpdateParameter>> fileTagDeleteJob = new(() => new(context));
        private readonly Lazy<OneWayJob<FileTagAddJob, FileTagUpdateParameter>> fileTagAddJob = new(() => new(context));
        private readonly Lazy<OneWayJob<GCCollectRunJob>> gcCollectRunJob = new(() => new(context));

        public readonly Lazy<TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileReadJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileLoadingJob = new(() => new(context));
        public readonly Lazy<OneWayJob<ImageFileCacheJob, ListParameter<string>>> ImageFileCacheJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>> ThumbnailsGetJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>> SubDirectoriesGetJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>> DirectoryViewHistoryGetJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>> AddressInfoGetJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<TagsGetJob, ListResult<string>>> TagsGetJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoGetJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<PipeServerJob, ValueResult<string>>> PipeServerJob = new(() => new(context));

        public readonly Lazy<TwoWayJob<FilesGetByDirectoryJob, ValueParameter<string>, DirectoryGetResult>> FilesGetByDirectoryJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>> FavoriteDirectoriesGetJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>>> FilesGetByRatingJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>> FilesGetByTagJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>> ImageFilesGetByDirectoryJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter<string>, ValueResult<string>>> NextDirectoryGetJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<MultiFilesExportJob, MultiFilesExportParameter, ValueResult<string>>> MultiFilesExportJob = new(() => new(context));
        public readonly Lazy<TwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>> BookmarksGetJob = new(() => new(context));

        private CommonJobs()
        {

        }

        ~CommonJobs()
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

            this.addBookmarkJob.Value.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartSingleFileExportJob(ISender sender, SingleFileExportParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.singleFileExportJob.Value.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryStateUpdateJob(ISender sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryStateUpdateJob.Value.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewHistoryAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryViewHistoryAddJob.Value.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartBookmarkDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.bookmarkDeleteJob.Value.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewCounterDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryViewCounterDeleteJob.Value.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartFileRatingUpdateJob(ISender sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileRatingUpdateJob.Value.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartFileTagDeleteJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileTagDeleteJob.Value.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartFileTagAddJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileTagAddJob.Value.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartGCCollectRunJob(ISender sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.gcCollectRunJob.Value.Initialize()
                .StartJob(sender);
        }
    }
}
