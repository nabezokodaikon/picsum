using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Common
{
    [SupportedOSPlatform("windows")]
    public sealed partial class JobCaller
        : IDisposable
    {
        private static SynchronizationContext? context = null;
        private static JobCaller? instance = null;

        public static JobCaller Instance
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
            instance = new JobCaller();
        }

        private bool disposed = false;

        private readonly Lazy<IOneWayJob<BookmarkAddJob, ValueParameter<string>>> addBookmarkJob
            = new(() => new OneWayJob<BookmarkAddJob, ValueParameter<string>>(context));
        private readonly Lazy<IOneWayJob<SingleFileExportJob, SingleFileExportParameter>> singleFileExportJob
            = new(() => new OneWayJob<SingleFileExportJob, SingleFileExportParameter>(context));
        private readonly Lazy<IOneWayJob<DirectoryStateUpdateJob, DirectoryStateParameter>> directoryStateUpdateJob
            = new(() => new OneWayJob<DirectoryStateUpdateJob, DirectoryStateParameter>(context));
        private readonly Lazy<IOneWayJob<DirectoryViewHistoryAddJob, ValueParameter<string>>> directoryViewHistoryAddJob
            = new(() => new OneWayJob<DirectoryViewHistoryAddJob, ValueParameter<string>>(context));
        private readonly Lazy<IOneWayJob<BookmarkDeleteJob, ListParameter<string>>> bookmarkDeleteJob
            = new(() => new OneWayJob<BookmarkDeleteJob, ListParameter<string>>(context));
        private readonly Lazy<IOneWayJob<DirectoryViewCounterDeleteJob, ListParameter<string>>> directoryViewCounterDeleteJob
            = new(() => new OneWayJob<DirectoryViewCounterDeleteJob, ListParameter<string>>(context));
        private readonly Lazy<IOneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter>> fileRatingUpdateJob
            = new(() => new OneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter>(context));
        private readonly Lazy<IOneWayJob<FileTagDeleteJob, FileTagUpdateParameter>> fileTagDeleteJob
            = new(() => new OneWayJob<FileTagDeleteJob, FileTagUpdateParameter>(context));
        private readonly Lazy<IOneWayJob<FileTagAddJob, FileTagUpdateParameter>> fileTagAddJob
            = new(() => new OneWayJob<FileTagAddJob, FileTagUpdateParameter>(context));
        private readonly Lazy<IOneWayJob<GCCollectRunJob>> gcCollectRunJob
            = new(() => new OneWayJob<GCCollectRunJob>(context));

        public readonly Lazy<ITwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileReadJob
            = new(() => new TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>(context));
        public readonly Lazy<ITwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileLoadingJob
            = new(() => new TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>(context));
        public readonly Lazy<IOneWayJob<ImageFileCacheJob, ListParameter<string>>> ImageFileCacheJob
            = new(() => new OneWayJob<ImageFileCacheJob, ListParameter<string>>(context));
        public readonly Lazy<ITwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>> ThumbnailsGetJob
            = new(() => new TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>(context));
        public readonly Lazy<ITwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>> SubDirectoriesGetJob
            = new(() => new TwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>> DirectoryViewHistoryGetJob
            = new(() => new TwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>> AddressInfoGetJob
            = new(() => new TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>(context));
        public readonly Lazy<ITwoWayJob<TagsGetJob, ListResult<string>>> TagsGetJob
            = new(() => new TwoWayJob<TagsGetJob, ListResult<string>>(context));
        public readonly Lazy<ITwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoGetJob
            = new(() => new TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>(context));
        public readonly Lazy<ITwoWayJob<PipeServerJob, ValueResult<string>>> PipeServerJob
            = new(() => new TwoWayJob<PipeServerJob, ValueResult<string>>(context));

        public readonly Lazy<ITwoWayJob<FilesGetByDirectoryJob, ValueParameter<string>, DirectoryGetResult>> FilesGetByDirectoryJob
            = new(() => new TwoWayJob<FilesGetByDirectoryJob, ValueParameter<string>, DirectoryGetResult>(context));
        public readonly Lazy<ITwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>> FavoriteDirectoriesGetJob
            = new(() => new TwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>>> FilesGetByRatingJob
            = new(() => new TwoWayJob<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>> FilesGetByTagJob
            = new(() => new TwoWayJob<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>(context));
        public readonly Lazy<ITwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>> ImageFilesGetByDirectoryJob
            = new(() => new TwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>(context));
        public readonly Lazy<ITwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter<string>, ValueResult<string>>> NextDirectoryGetJob
            = new(() => new TwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter<string>, ValueResult<string>>(context));
        public readonly Lazy<ITwoWayJob<MultiFilesExportJob, MultiFilesExportParameter, ValueResult<string>>> MultiFilesExportJob
            = new(() => new TwoWayJob<MultiFilesExportJob, MultiFilesExportParameter, ValueResult<string>>(context));
        public readonly Lazy<ITwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>> BookmarksGetJob
            = new(() => new TwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>(context));

        private JobCaller()
        {

        }

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
