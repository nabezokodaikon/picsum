using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Job;

namespace PicSum.Job.Common
{

    public sealed partial class JobCaller
        : IDisposable
    {
        private bool _disposed = false;

        private readonly Lazy<OneWayJobQueue> _oneWayJobQueue = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<TwoWayJobQueue> _twoWayJobQueue = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);

        public readonly Lazy<TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileReadJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileLoadingJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<OneWayJob<ImageFileCacheJob, ImageFileCacheParameter>> ImageFileCacheJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>> ThumbnailsGetJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<TakenDatesGetJob, TakenDatesGetParameter, TakenDateResult>> TakenDatesGetJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoGetJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<FileDeepInfoLoadingJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoLoadingJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>> AddressInfoGetJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<TwoWayJob<PipeServerJob, ValueResult<string>>> PipeServerJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);
        public readonly Lazy<OneWayJob<GCCollectRunJob>> GCCollectRunJob = new(
            static () => new(), LazyThreadSafetyMode.ExecutionAndPublication);

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._oneWayJobQueue.Value.Dispose();
            this._twoWayJobQueue.Value.Dispose();

            this.ImageFileReadJob.Value.Dispose();
            this.ImageFileLoadingJob.Value.Dispose();
            this.ImageFileCacheJob.Value.Dispose();
            this.ThumbnailsGetJob.Value.Dispose();
            this.TakenDatesGetJob.Value.Dispose();
            this.FileDeepInfoGetJob.Value.Dispose();
            this.FileDeepInfoLoadingJob.Value.Dispose();
            this.AddressInfoGetJob.Value.Dispose();
            this.PipeServerJob.Value.Dispose();
            this.GCCollectRunJob.Value.Dispose();

            this._disposed = true;

            GC.SuppressFinalize(this);
        }

        public async ValueTask EnqueueBookmarkUpdateJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            await this._oneWayJobQueue.Value.Enqueue<BookmarkUpdateJob, ValueParameter<string>>(sender, parameter);
        }

        public async ValueTask EnqueueDirectoryStateUpdateJob(ISender sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            await this._oneWayJobQueue.Value.Enqueue<DirectoryStateUpdateJob, DirectoryStateParameter>(sender, parameter);
        }

        public async ValueTask EnqueueDirectoryViewHistoryAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            await this._oneWayJobQueue.Value.Enqueue<DirectoryViewHistoryUpdateJob, ValueParameter<string>>(sender, parameter);
        }

        public async ValueTask EnqueueDirectoryViewCounterIncrementJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            await this._oneWayJobQueue.Value.Enqueue<DirectoryViewCounterIncrementJob, ValueParameter<string>>(sender, parameter);
        }

        public async ValueTask EnqueueBookmarkDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            await this._oneWayJobQueue.Value.Enqueue<BookmarkDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public async ValueTask EnqueueDirectoryViewCounterDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            await this._oneWayJobQueue.Value.Enqueue<DirectoryViewCounterDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public async ValueTask EnqueueFileRatingUpdateJob(ISender sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            await this._oneWayJobQueue.Value.Enqueue<FileRatingUpdateJob, FileRatingUpdateParameter>(sender, parameter);
        }

        public async ValueTask EnqueueFileTagDeleteJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            await this._oneWayJobQueue.Value.Enqueue<FileTagDeleteJob, FileTagUpdateParameter>(sender, parameter);
        }

        public async ValueTask EnqueueFileTagUpdateJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            await this._oneWayJobQueue.Value.Enqueue<FileTagUpdateJob, FileTagUpdateParameter>(sender, parameter);
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
