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
        public readonly static CommonJobs Instance = new();

        private bool disposed = false;

        private OneWayJob<BookmarkAddJob, ValueParameter<string>>? addBookmarkJob = null;
        private OneWayJob<SingleFileExportJob, SingleFileExportParameter>? singleFileExportJob = null;
        private OneWayJob<DirectoryStateUpdateJob, DirectoryStateParameter>? directoryStateUpdateJob = null;
        private OneWayJob<DirectoryViewHistoryAddJob, ValueParameter<string>>? directoryViewHistoryAddJob = null;
        private OneWayJob<BookmarkDeleteJob, ListParameter<string>>? bookmarkDeleteJob = null;
        private OneWayJob<DirectoryViewCounterDeleteJob, ListParameter<string>>? directoryViewCounterDeleteJob = null;
        private OneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter>? fileRatingUpdateJob = null;
        private OneWayJob<FileTagDeleteJob, FileTagUpdateParameter>? fileTagDeleteJob = null;
        private OneWayJob<FileTagAddJob, FileTagUpdateParameter>? fileTagAddJob = null;
        private OneWayJob<GCCollectRunJob>? gcCollectRunJob = null;

        private TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult>? imageFileReadJob = null;
        private TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult>? imageFileLoadingJob = null;
        private OneWayJob<ImageFileCacheJob, ListParameter<string>>? imageFileCacheJob = null;
        private TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult>? thumbnailsGetJob = null;
        private TwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>? subDirectoriesGetJob = null;
        private TwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>>? directoryViewHistoryGetJob = null;
        private TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult>? addressInfoGetJob = null;
        private TwoWayJob<TagsGetJob, ListResult<string>>? tagsGetJob = null;
        private TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult>? fileDeepInfoGetJob = null;
        private TwoWayJob<PipeServerJob, ValueResult<string>>? pipeServerJob = null;

        private TwoWayJob<FilesGetByDirectoryJob, ValueParameter<string>, DirectoryGetResult>? filesGetByDirectoryJob = null;
        private TwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>? favoriteDirectoriesGetJob = null;
        private TwoWayJob<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>>? filesGetByRatingJob = null;
        private TwoWayJob<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>? filesGetByTagJob = null;
        private TwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>? imageFilesGetByDirectoryJob = null;
        private TwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter<string>, ValueResult<string>>? nextDirectoryGetJob = null;
        private TwoWayJob<MultiFilesExportJob, MultiFilesExportParameter, ValueResult<string>>? multiFilesExportJob = null;
        private TwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>? bookmarksGetJob = null;

        public TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult> ImageFileReadJob => this.imageFileReadJob ??= new();
        public TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult> ImageFileLoadingJob => this.imageFileLoadingJob ??= new();
        public OneWayJob<ImageFileCacheJob, ListParameter<string>> ImageFileCacheJob => this.imageFileCacheJob ??= new();
        public TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult> ThumbnailsGetJob => this.thumbnailsGetJob ??= new();
        public TwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>> SubDirectoriesGetJob => this.subDirectoriesGetJob ??= new();
        public TwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>> DirectoryViewHistoryGetJob => this.directoryViewHistoryGetJob ??= new();
        public TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult> AddressInfoGetJob => this.addressInfoGetJob ??= new();
        public TwoWayJob<TagsGetJob, ListResult<string>> TagsGetJob => this.tagsGetJob ??= new();
        public TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult> FileDeepInfoGetJob => this.fileDeepInfoGetJob ??= new();
        public TwoWayJob<PipeServerJob, ValueResult<string>> PipeServerJob => this.pipeServerJob ??= new();

        public TwoWayJob<FilesGetByDirectoryJob, ValueParameter<string>, DirectoryGetResult>? FilesGetByDirectoryJob => this.filesGetByDirectoryJob ??= new();
        public TwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>? FavoriteDirectoriesGetJob => this.favoriteDirectoriesGetJob ??= new();
        public TwoWayJob<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>>? FilesGetByRatingJob => this.filesGetByRatingJob ??= new();
        public TwoWayJob<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>? FilesGetByTagJob => this.filesGetByTagJob ??= new();
        public TwoWayJob<ImageFilesGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>? ImageFilesGetByDirectoryJob => this.imageFilesGetByDirectoryJob ??= new();
        public TwoWayJob<NextDirectoryGetJob, NextDirectoryGetParameter<string>, ValueResult<string>>? NextDirectoryGetJob => this.nextDirectoryGetJob ??= new();
        public TwoWayJob<MultiFilesExportJob, MultiFilesExportParameter, ValueResult<string>>? MultiFilesExportJob => this.multiFilesExportJob ??= new();
        public TwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>>? BookmarksGetJob => this.bookmarksGetJob ??= new();

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
                this.addBookmarkJob?.Dispose();
                this.singleFileExportJob?.Dispose();
                this.directoryStateUpdateJob?.Dispose();
                this.directoryViewHistoryAddJob?.Dispose();
                this.bookmarkDeleteJob?.Dispose();
                this.directoryViewCounterDeleteJob?.Dispose();
                this.fileRatingUpdateJob?.Dispose();
                this.fileTagDeleteJob?.Dispose();
                this.fileTagAddJob?.Dispose();
                this.gcCollectRunJob?.Dispose();

                this.imageFileReadJob?.Dispose();
                this.imageFileLoadingJob?.Dispose();
                this.imageFileCacheJob?.Dispose();
                this.thumbnailsGetJob?.Dispose();
                this.subDirectoriesGetJob?.Dispose();
                this.directoryViewHistoryGetJob?.Dispose();
                this.addressInfoGetJob?.Dispose();
                this.tagsGetJob?.Dispose();
                this.fileDeepInfoGetJob?.Dispose();
                this.pipeServerJob?.Dispose();

                this.filesGetByDirectoryJob?.Dispose();
                this.favoriteDirectoriesGetJob?.Dispose();
                this.filesGetByRatingJob?.Dispose();
                this.filesGetByTagJob?.Dispose();
                this.imageFilesGetByDirectoryJob?.Dispose();
                this.nextDirectoryGetJob?.Dispose();
                this.multiFilesExportJob?.Dispose();
                this.bookmarksGetJob?.Dispose();
            }

            this.disposed = true;
        }

        public void StartBookmarkAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.addBookmarkJob ??= new();
            this.addBookmarkJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartSingleFileExportJob(ISender sender, SingleFileExportParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.singleFileExportJob ??= new();
            this.singleFileExportJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryStateUpdateJob(ISender sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryStateUpdateJob ??= new();
            this.directoryStateUpdateJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewHistoryAddJob(ISender sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryViewHistoryAddJob ??= new();
            this.directoryViewHistoryAddJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartBookmarkDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.bookmarkDeleteJob ??= new();
            this.bookmarkDeleteJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewCounterDeleteJob(ISender sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryViewCounterDeleteJob ??= new();
            this.directoryViewCounterDeleteJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartFileRatingUpdateJob(ISender sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileRatingUpdateJob ??= new();
            this.fileRatingUpdateJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartFileTagDeleteJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileTagDeleteJob ??= new();
            this.fileTagDeleteJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartFileTagAddJob(ISender sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileTagAddJob ??= new();
            this.fileTagAddJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartGCCollectRunJob(ISender sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.gcCollectRunJob ??= new();
            this.gcCollectRunJob.Initialize()
                .StartJob(sender);
        }
    }
}
