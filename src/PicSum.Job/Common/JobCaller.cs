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

        private readonly FastLazy<JobQueue> _jobQueue = new(() => new JobQueue());

        public readonly FastLazy<ITwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileReadJob
            = new(() => new TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>> ImageFileLoadingJob
            = new(() => new TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>(context, new JobTask()));
        public readonly FastLazy<IOneWayJob<ImageFileCacheJob, ImageFileCacheParameter>> ImageFileCacheJob
            = new(() => new OneWayJob<ImageFileCacheJob, ImageFileCacheParameter>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>> ThumbnailsGetJob
            = new(() => new TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>> SubDirectoriesGetJob
            = new(() => new TwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>> DirectoryViewHistoryGetJob
            = new(() => new TwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>> AddressInfoGetJob
            = new(() => new TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<TagsGetJob, ListResult<string>>> TagsGetJob
            = new(() => new TwoWayJob<TagsGetJob, ListResult<string>>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoGetJob
            = new(() => new TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<FileDeepInfoLoadingJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>> FileDeepInfoLoadingJob
            = new(() => new TwoWayJob<FileDeepInfoLoadingJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<PipeServerJob, ValueResult<string>>> PipeServerJob
            = new(() => new TwoWayJob<PipeServerJob, ValueResult<string>>(context, new JobTask()));
        public readonly FastLazy<IOneWayJob<GCCollectRunJob>> GCCollectRunJob
            = new(() => new OneWayJob<GCCollectRunJob>(context, new JobTask()));

        public readonly FastLazy<ITwoWayJob<FilesGetByDirectoryJob, FilesGetByDirectoryParameter, DirectoryGetResult>> FilesGetByDirectoryJob
            = new(() => new TwoWayJob<FilesGetByDirectoryJob, FilesGetByDirectoryParameter, DirectoryGetResult>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>> FavoriteDirectoriesGetJob
            = new(() => new TwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<FilesGetByRatingJob, FilesGetByRatingParameter, ListResult<FileShallowInfoEntity>>> FilesGetByRatingJob
            = new(() => new TwoWayJob<FilesGetByRatingJob, FilesGetByRatingParameter, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<FilesGetByTagJob, FilesGetByTagParameter, ListResult<FileShallowInfoEntity>>> FilesGetByTagJob
            = new(() => new TwoWayJob<FilesGetByTagJob, FilesGetByTagParameter, ListResult<FileShallowInfoEntity>>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>> ImageFilesGetByDirectoryJob
            = new(() => new TwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter, ValueResult<string>>> NextDirectoryGetJob
            = new(() => new TwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter, ValueResult<string>>(context, new JobTask()));
        public readonly FastLazy<ITwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>> BookmarksGetJob
            = new(() => new TwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>(context, new JobTask()));

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
