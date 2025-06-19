using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Common
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class JobCaller(SynchronizationContext context)
        : IDisposable
    {
        private bool _disposed = false;

        private readonly FastLazy<JobQueue> _jobQueue = new(() => new());

        public readonly FastLazy<TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileReadJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileLoadingJob = new(() => new(context));
        public readonly FastLazy<OneWayJob<ImageFileCacheJob, ImageFileCacheParameter>> ImageFileCacheJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>> ThumbnailsGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>> SubDirectoriesGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>> DirectoryViewHistoryGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>> AddressInfoGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<TagsGetJob, ListResult<string>>> TagsGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<FileDeepInfoLoadingJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoLoadingJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<PipeServerJob, ValueResult<string>>> PipeServerJob = new(() => new(context));
        public readonly FastLazy<OneWayJob<GCCollectRunJob>> GCCollectRunJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<FilesGetByDirectoryJob, FilesGetByDirectoryParameter, DirectoryGetResult>> FilesGetByDirectoryJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>> FavoriteDirectoriesGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<FilesGetByRatingJob, FilesGetByRatingParameter, ListResult<FileShallowInfoEntity>>> FilesGetByRatingJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<FilesGetByTagJob, FilesGetByTagParameter, ListResult<FileShallowInfoEntity>>> FilesGetByTagJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>> ImageFilesGetByDirectoryJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter, ValueResult<string>>> NextDirectoryGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>> BookmarksGetJob = new(() => new(context));

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._jobQueue.Dispose();

                this.ImageFileReadJob.Dispose();
                this.ImageFileLoadingJob.Dispose();
                this.ImageFileCacheJob.Dispose();
                this.ThumbnailsGetJob.Dispose();
                this.SubDirectoriesGetJob.Dispose();
                this.DirectoryViewHistoryGetJob.Dispose();
                this.AddressInfoGetJob.Dispose();
                this.TagsGetJob.Dispose();
                this.FileDeepInfoGetJob.Dispose();
                this.FileDeepInfoLoadingJob.Dispose();
                this.PipeServerJob.Dispose();
                this.GCCollectRunJob.Dispose();
                this.FilesGetByDirectoryJob.Dispose();
                this.FavoriteDirectoriesGetJob.Dispose();
                this.FilesGetByRatingJob.Dispose();
                this.FilesGetByTagJob.Dispose();
                this.ImageFilesGetByDirectoryJob.Dispose();
                this.NextDirectoryGetJob.Dispose();
                this.BookmarksGetJob.Dispose();
            }

            this._disposed = true;
        }

        public void StartBookmarkAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<BookmarkAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void StartDirectoryStateUpdateJob(ISender sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<DirectoryStateUpdateJob, DirectoryStateParameter>(sender, parameter);
        }

        public void StartDirectoryViewHistoryAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<DirectoryViewHistoryAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void StartBookmarkDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this._jobQueue.Value.Enqueue<BookmarkDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void StartDirectoryViewCounterDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this._jobQueue.Value.Enqueue<DirectoryViewCounterDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void StartFileRatingUpdateJob(ISender sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<FileRatingUpdateJob, FileRatingUpdateParameter>(sender, parameter);
        }

        public void StartFileTagDeleteJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<FileTagDeleteJob, FileTagUpdateParameter>(sender, parameter);
        }

        public void StartFileTagAddJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<FileTagAddJob, FileTagUpdateParameter>(sender, parameter);
        }
    }
}
