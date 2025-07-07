using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.ConsoleAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Common
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class JobCaller
        : IAsyncDisposable
    {
        private bool _disposed = false;

        private OneWayJobQueue _oneWayQueue;
        private TwoWayJobQueue _twoWayJobQueue;

        public TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult> ImageFileReadJob { get; private set; }
        public TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult> ImageFileLoadingJob { get; private set; }
        public OneWayJob<ImageFileCacheJob, ImageFileCacheParameter> ImageFileCacheJob { get; private set; }
        public TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult> ThumbnailsGetJob { get; private set; }
        public TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult> FileDeepInfoGetJob { get; private set; }
        public TwoWayJob<FileDeepInfoLoadingJob, FileDeepInfoGetParameter, FileDeepInfoGetResult> FileDeepInfoLoadingJob { get; private set; }
        public TwoWayJob<PipeServerJob, ValueResult<string>> PipeServerJob { get; private set; }
        public OneWayJob<GCCollectRunJob> GCCollectRunJob { get; private set; }

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。'required' 修飾子を追加するか、Null 許容として宣言することを検討してください。
        public JobCaller(SynchronizationContext context)
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。'required' 修飾子を追加するか、Null 許容として宣言することを検討してください。
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            using (TimeMeasuring.Run(true, "JobCaller.New"))
            {
                Action[] actions = [
                    () => this._oneWayQueue = new(),
                    () => this._twoWayJobQueue = new(context),
                    () => this.ImageFileReadJob = new(context),
                    () => this.ImageFileLoadingJob = new(context),
                    () => this.ImageFileCacheJob = new(context),
                    () => this.ThumbnailsGetJob = new(context),
                    () => this.FileDeepInfoGetJob = new(context),
                    () => this.FileDeepInfoLoadingJob = new(context),
                    () => this.PipeServerJob = new(context),
                    () => this.GCCollectRunJob = new(context),
                ];

                Parallel.ForEach(
                    actions,
                    new ParallelOptions { MaxDegreeOfParallelism = actions.Length },
                    _ => _()
                );
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (this._disposed)
            {
                return;
            }

            await this._oneWayQueue.DisposeAsync();
            await this._twoWayJobQueue.DisposeAsync();

            await this.ImageFileReadJob.DisposeAsync();
            await this.ImageFileLoadingJob.DisposeAsync();
            await this.ImageFileCacheJob.DisposeAsync();
            await this.ThumbnailsGetJob.DisposeAsync();
            await this.FileDeepInfoGetJob.DisposeAsync();
            await this.FileDeepInfoLoadingJob.DisposeAsync();
            await this.PipeServerJob.DisposeAsync();
            await this.GCCollectRunJob.DisposeAsync();

            this._disposed = true;

            GC.SuppressFinalize(this);
        }

        public void EnqueueBookmarkAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Enqueue<BookmarkAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void EnqueueDirectoryStateUpdateJob(ISender sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Enqueue<DirectoryStateUpdateJob, DirectoryStateParameter>(sender, parameter);
        }

        public void EnqueueDirectoryViewHistoryAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Enqueue<DirectoryViewHistoryAddJob, ValueParameter<string>>(sender, parameter);
        }

        public void EnqueueBookmarkDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this._oneWayQueue.Enqueue<BookmarkDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void EnqueueDirectoryViewCounterDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this._oneWayQueue.Enqueue<DirectoryViewCounterDeleteJob, ListParameter<string>>(sender, parameter);
        }

        public void EnqueueFileRatingUpdateJob(ISender sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Enqueue<FileRatingUpdateJob, FileRatingUpdateParameter>(sender, parameter);
        }

        public void EnqueueFileTagDeleteJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Enqueue<FileTagDeleteJob, FileTagUpdateParameter>(sender, parameter);
        }

        public void EnqueueFileTagAddJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._oneWayQueue.Enqueue<FileTagAddJob, FileTagUpdateParameter>(sender, parameter);
        }

        public void EnqueueTagsGetJob(
            ISender sender, Action<ListResult<string>> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(sender, nameof(callback));

            this._twoWayJobQueue
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

            this._twoWayJobQueue
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

            this._twoWayJobQueue
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

            this._twoWayJobQueue
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

            this._twoWayJobQueue
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

            this._twoWayJobQueue
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

            this._twoWayJobQueue
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

            this._twoWayJobQueue
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

            this._twoWayJobQueue
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

            this._twoWayJobQueue
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

            this._twoWayJobQueue
                .Enqueue<
                    FavoriteDirectoriesGetJob,
                    FavoriteDirectoriesGetParameter,
                    ListResult<FileShallowInfoEntity>>(
                sender, parameter, callback);
        }
    }
}
