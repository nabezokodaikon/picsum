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
        : IAsyncDisposable
    {
        private bool _disposed = false;

        private readonly Lazy<OneWayJobQueue> _oneWayQueue = new(
            () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<TwoWayJobQueue> _twoWayJobQueue = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);

        public readonly Lazy<TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileReadJob = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileLoadingJob = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<OneWayJob<ImageFileCacheJob, ImageFileCacheParameter>> ImageFileCacheJob = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>> ThumbnailsGetJob = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoGetJob = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<FileDeepInfoLoadingJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoLoadingJob = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>> AddressInfoGetJob = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<PipeServerJob, ValueResult<string>>> PipeServerJob = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<OneWayJob<GCCollectRunJob>> GCCollectRunJob = new(
            () => new(context), LazyThreadSafetyMode.ExecutionAndPublication);

        public async ValueTask DisposeAsync()
        {
            if (this._disposed)
            {
                return;
            }

            await this._oneWayQueue.Value.DisposeAsync();
            await this._twoWayJobQueue.Value.DisposeAsync();

            await this.ImageFileReadJob.Value.DisposeAsync();
            await this.ImageFileLoadingJob.Value.DisposeAsync();
            await this.ImageFileCacheJob.Value.DisposeAsync();
            await this.ThumbnailsGetJob.Value.DisposeAsync();
            await this.FileDeepInfoGetJob.Value.DisposeAsync();
            await this.FileDeepInfoLoadingJob.Value.DisposeAsync();
            await this.AddressInfoGetJob.Value.DisposeAsync();
            await this.PipeServerJob.Value.DisposeAsync();
            await this.GCCollectRunJob.Value.DisposeAsync();

            this._disposed = true;

            GC.SuppressFinalize(this);
        }

        public void EnqueueBookmarkAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Value.Enqueue<BookmarkAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void EnqueueDirectoryStateUpdateJob(ISender sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Value.Enqueue<DirectoryStateUpdateJob, DirectoryStateParameter>(sender, parameter);
        }

        public void EnqueueDirectoryViewHistoryAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Value.Enqueue<DirectoryViewHistoryAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void EnqueueBookmarkDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this._oneWayQueue.Value.Enqueue<BookmarkDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void EnqueueDirectoryViewCounterDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this._oneWayQueue.Value.Enqueue<DirectoryViewCounterDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void EnqueueFileRatingUpdateJob(ISender sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Value.Enqueue<FileRatingUpdateJob, FileRatingUpdateParameter>(sender, parameter);
        }

        public void EnqueueFileTagDeleteJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Value.Enqueue<FileTagDeleteJob, FileTagUpdateParameter>(sender, parameter);
        }

        public void EnqueueFileTagAddJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Value.Enqueue<FileTagAddJob, FileTagUpdateParameter>(sender, parameter);
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
