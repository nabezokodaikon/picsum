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

        private readonly Lazy<JobQueue> jobQueue = new(() => new JobQueue());

        public readonly Lazy<ITwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileReadJob
            = new(() => new TwoWayThread<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>(context, new JobThread()));
        public readonly Lazy<ITwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileLoadingJob
            = new(() => new TwoWayThread<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>(context, new JobThread()));
        public readonly Lazy<IOneWayJob<ImageFileCacheJob, ListParameter<string>>> ImageFileCacheJob
            = new(() => new OneWayThread<ImageFileCacheJob, ListParameter<string>>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>> ThumbnailsGetJob
            = new(() => new TwoWayThread<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>(context, new JobThread()));
        public readonly Lazy<ITwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>> SubDirectoriesGetJob
            = new(() => new TwoWayThread<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>> DirectoryViewHistoryGetJob
            = new(() => new TwoWayThread<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>> AddressInfoGetJob
            = new(() => new TwoWayThread<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<TagsGetJob, ListResult<string>>> TagsGetJob
            = new(() => new TwoWayThread<TagsGetJob, ListResult<string>>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoGetJob
            = new(() => new TwoWayThread<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<FileDeepInfoLoadingJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoLoadingJob
            = new(() => new TwoWayThread<FileDeepInfoLoadingJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<PipeServerJob, ValueResult<string>>> PipeServerJob
            = new(() => new TwoWayThread<PipeServerJob, ValueResult<string>>(context, new JobTask()));
        public readonly Lazy<IOneWayJob<GCCollectRunJob>> GCCollectRunJob
            = new(() => new OneWayThread<GCCollectRunJob>(context, new JobTask()));

        public readonly Lazy<ITwoWayJob<FilesGetByDirectoryJob, FilesGetByDirectoryParameter, DirectoryGetResult>> FilesGetByDirectoryJob
            = new(() => new TwoWayThread<FilesGetByDirectoryJob, FilesGetByDirectoryParameter, DirectoryGetResult>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>> FavoriteDirectoriesGetJob
            = new(() => new TwoWayThread<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<FilesGetByRatingJob, FilesGetByRatingParameter, ListResult<FileShallowInfoEntity>>> FilesGetByRatingJob
            = new(() => new TwoWayThread<FilesGetByRatingJob, FilesGetByRatingParameter, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<FilesGetByTagJob, FilesGetByTagParameter, ListResult<FileShallowInfoEntity>>> FilesGetByTagJob
            = new(() => new TwoWayThread<FilesGetByTagJob, FilesGetByTagParameter, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>> ImageFilesGetByDirectoryJob
            = new(() => new TwoWayThread<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter<string>, ValueResult<string>>> NextDirectoryGetJob
            = new(() => new TwoWayThread<NextDirectoryGetJob, NextDirectoryGetParameter<string>, ValueResult<string>>(context, new JobTask()));
        public readonly Lazy<ITwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>> BookmarksGetJob
            = new(() => new TwoWayThread<BookmarksGetJob, ListResult<FileShallowInfoEntity>>(context, new JobTask()));

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
                this.jobQueue.Value.Dispose();

                this.ImageFileReadJob.Value.Dispose();
                this.ImageFileLoadingJob.Value.Dispose();
                this.ImageFileCacheJob.Value.Dispose();
                this.ThumbnailsGetJob.Value.Dispose();
                this.SubDirectoriesGetJob.Value.Dispose();
                this.DirectoryViewHistoryGetJob.Value.Dispose();
                this.AddressInfoGetJob.Value.Dispose();
                this.TagsGetJob.Value.Dispose();
                this.FileDeepInfoGetJob.Value.Dispose();
                this.FileDeepInfoLoadingJob.Value?.Dispose();
                this.PipeServerJob.Value.Dispose();
                this.GCCollectRunJob.Value.Dispose();

                this.FilesGetByDirectoryJob.Value.Dispose();
                this.FavoriteDirectoriesGetJob.Value.Dispose();
                this.FilesGetByRatingJob.Value.Dispose();
                this.FilesGetByTagJob.Value.Dispose();
                this.ImageFilesGetByDirectoryJob.Value.Dispose();
                this.NextDirectoryGetJob.Value.Dispose();
                this.BookmarksGetJob.Value.Dispose();
            }

            this.disposed = true;
        }

        public void StartBookmarkAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.jobQueue.Value.Enqueue<BookmarkAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void StartDirectoryStateUpdateJob(ISender sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.jobQueue.Value.Enqueue<DirectoryStateUpdateJob, DirectoryStateParameter>(sender, parameter);
        }

        public void StartDirectoryViewHistoryAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.jobQueue.Value.Enqueue<DirectoryViewHistoryAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void StartBookmarkDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.jobQueue.Value.Enqueue<BookmarkDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void StartDirectoryViewCounterDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.jobQueue.Value.Enqueue<DirectoryViewCounterDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void StartFileRatingUpdateJob(ISender sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.jobQueue.Value.Enqueue<FileRatingUpdateJob, FileRatingUpdateParameter>(sender, parameter);
        }

        public void StartFileTagDeleteJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.jobQueue.Value.Enqueue<FileTagDeleteJob, FileTagUpdateParameter>(sender, parameter);
        }

        public void StartFileTagAddJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.jobQueue.Value.Enqueue<FileTagAddJob, FileTagUpdateParameter>(sender, parameter);
        }
    }
}
