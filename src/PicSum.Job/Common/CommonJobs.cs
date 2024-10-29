using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Job;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Job.Common
{
    [SupportedOSPlatform("windows")]
    public sealed partial class CommonJobs
        : IDisposable
    {
        public static CommonJobs Instance = new();

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
            }

            this.disposed = true;
        }

        public void StartBookmarkAddJob(Control sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.addBookmarkJob ??= new();
            this.addBookmarkJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartSingleFileExportJob(Control sender, SingleFileExportParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.singleFileExportJob ??= new();
            this.singleFileExportJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryStateUpdateJob(Control sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryStateUpdateJob ??= new();
            this.directoryStateUpdateJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewHistoryAddJob(Control sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryViewHistoryAddJob ??= new();
            this.directoryViewHistoryAddJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartBookmarkDeleteJob(Control sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.bookmarkDeleteJob ??= new();
            this.bookmarkDeleteJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewCounterDeleteJob(Control sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.directoryViewCounterDeleteJob ??= new();
            this.directoryViewCounterDeleteJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartFileRatingUpdateJob(Control sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileRatingUpdateJob ??= new();
            this.fileRatingUpdateJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartFileTagDeleteJob(Control sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileTagDeleteJob ??= new();
            this.fileTagDeleteJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartFileTagAddJob(Control sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.fileTagAddJob ??= new();
            this.fileTagAddJob.Initialize()
                .StartJob(sender, parameter);
        }

        public void StartGCCollectRunJob(Control sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.gcCollectRunJob ??= new();
            this.gcCollectRunJob.Initialize()
                .StartJob(sender);
        }
    }
}
