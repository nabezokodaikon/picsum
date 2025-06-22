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
        private readonly FastLazy<TwoWayJobQueue> _twoWayJobQueue = new(() => new(context));

        public readonly FastLazy<TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileReadJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileLoadingJob = new(() => new(context));
        public readonly FastLazy<OneWayJob<ImageFileCacheJob, ImageFileCacheParameter>> ImageFileCacheJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>> ThumbnailsGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoGetJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<FileDeepInfoLoadingJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoLoadingJob = new(() => new(context));
        public readonly FastLazy<TwoWayJob<PipeServerJob, ValueResult<string>>> PipeServerJob = new(() => new(context));
        public readonly FastLazy<OneWayJob<GCCollectRunJob>> GCCollectRunJob = new(() => new(context));

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
                this._twoWayJobQueue.Dispose();

                this.ImageFileReadJob.Dispose();
                this.ImageFileLoadingJob.Dispose();
                this.ImageFileCacheJob.Dispose();
                this.ThumbnailsGetJob.Dispose();
                this.FileDeepInfoGetJob.Dispose();
                this.FileDeepInfoLoadingJob.Dispose();
                this.PipeServerJob.Dispose();
                this.GCCollectRunJob.Dispose();
            }

            this._disposed = true;
        }

        public void EnqueueBookmarkAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<BookmarkAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void EnqueueDirectoryStateUpdateJob(ISender sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<DirectoryStateUpdateJob, DirectoryStateParameter>(sender, parameter);
        }

        public void EnqueueDirectoryViewHistoryAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<DirectoryViewHistoryAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void EnqueueBookmarkDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this._jobQueue.Value.Enqueue<BookmarkDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void EnqueueDirectoryViewCounterDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this._jobQueue.Value.Enqueue<DirectoryViewCounterDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void EnqueueFileRatingUpdateJob(ISender sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<FileRatingUpdateJob, FileRatingUpdateParameter>(sender, parameter);
        }

        public void EnqueueFileTagDeleteJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<FileTagDeleteJob, FileTagUpdateParameter>(sender, parameter);
        }

        public void EnqueueFileTagAddJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._jobQueue.Value.Enqueue<FileTagAddJob, FileTagUpdateParameter>(sender, parameter);
        }

        public void EnqueueTagsGetJob(
            ISender sender, Action<ListResult<string>> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    TagsGetJob,
                    ListResult<string>>(
                sender, callback);
        }

        public void EnqueueSubDirectoriesGetJob(
            ISender sender,
            ValueParameter<string> parameter,
            Action<ListResult<FileShallowInfoEntity>> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    SubDirectoriesGetJob,
                    ValueParameter<string>,
                    ListResult<FileShallowInfoEntity>>(
                sender, parameter, callback);
        }

        public void EnqueueDirectoryViewHistoryGetJob(
            ISender sender, Action<ListResult<FileShallowInfoEntity>> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    DirectoryViewHistoryGetJob,
                    ListResult<FileShallowInfoEntity>>(
                sender, callback);
        }

        public void EnqueueAddressInfoGetJob(
            ISender sender,
            ValueParameter<string> parameter,
            Action<AddressInfoGetResult> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    AddressInfoGetJob,
                    ValueParameter<string>,
                    AddressInfoGetResult>(
                sender, parameter, callback);
        }

        public void EnqueueNextDirectoryGetJob(
            ISender sender,
            NextDirectoryGetParameter parameter,
            Action<ValueResult<string>> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    NextDirectoryGetJob,
                    NextDirectoryGetParameter,
                    ValueResult<string>>(
                sender, parameter, callback);
        }

        public void EnqueueFilesGetByDirectoryJob(
            ISender sender,
            FilesGetByDirectoryParameter parameter,
            Action<DirectoryGetResult> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    FilesGetByDirectoryJob,
                    FilesGetByDirectoryParameter,
                    DirectoryGetResult>(
                sender, parameter, callback);
        }

        public void EnqueueBookmarksGetJob(
            ISender sender, Action<ListResult<FileShallowInfoEntity>> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    BookmarksGetJob,
                    ListResult<FileShallowInfoEntity>>(
                sender, callback);
        }

        public void EnqueueFilesGetByTagJob(
            ISender sender,
            FilesGetByTagParameter parameter,
            Action<ListResult<FileShallowInfoEntity>> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    FilesGetByTagJob,
                    FilesGetByTagParameter,
                    ListResult<FileShallowInfoEntity>>(
                sender, parameter, callback);
        }

        public void EnqueueImageFilesGetByDirectoryJob(
            ISender sender,
            ImageFileGetByDirectoryParameter parameter,
            Action<ImageFilesGetByDirectoryResult> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    ImageFilesGetByDirectoryJob,
                    ImageFileGetByDirectoryParameter,
                    ImageFilesGetByDirectoryResult>(
                sender, parameter, callback);
        }

        public void EnqueueFilesGetByRatingJob(
            ISender sender,
            FilesGetByRatingParameter parameter,
            Action<ListResult<FileShallowInfoEntity>> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    FilesGetByRatingJob,
                    FilesGetByRatingParameter,
                    ListResult<FileShallowInfoEntity>>(
                sender, parameter, callback);
        }

        public void EnqueueFavoriteDirectoriesGetJob(
            ISender sender,
            FavoriteDirectoriesGetParameter parameter,
            Action<ListResult<FileShallowInfoEntity>> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue.Value
                .Enqueue<
                    FavoriteDirectoriesGetJob,
                    FavoriteDirectoriesGetParameter,
                    ListResult<FileShallowInfoEntity>>(
                sender, parameter, callback);
        }
    }
}
