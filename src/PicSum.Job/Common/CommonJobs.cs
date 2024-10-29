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
        private OneWayJob<DirectoryViewHistoryAddJob, ValueParameter<string>>? directoryHistoryaddJob = null;
        private OneWayJob<BookmarkDeleteJob, ListParameter<string>>? bookmarkDeleteJob = null;
        private OneWayJob<DirectoryViewCounterDeleteJob, ListParameter<string>>? directoryViewCounterDeleteJob = null;
        private OneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter>? fileRatingUpdateJob = null;
        private OneWayJob<FileTagDeleteJob, FileTagUpdateParameter>? fileTagDeleteJob = null;
        private OneWayJob<FileTagAddJob, FileTagUpdateParameter>? fileTagAddJob = null;
        private OneWayJob<GCCollectRunJob>? gcCollectRunJob = null;

        public TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult> ImageFileReadJob { get; private set; } = new();
        public TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult> ImageFileLoadingJob { get; private set; } = new();
        public OneWayJob<ImageFileCacheJob, ListParameter<string>> ImageFileCacheJob { get; private set; } = new();
        public TwoWayJob<ThumbnailsGetJob, ThumbnailsGetParameter, ThumbnailImageResult> ThumbnailsGetJob { get; private set; } = new();
        public TwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>> SubDirectoriesGetJob { get; private set; } = new();
        public TwoWayJob<DirectoryViewHistoryGetJob, ListResult<FileShallowInfoEntity>> DirectoryViewHistoryGetJob { get; private set; } = new();
        public TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult> AddressInfoGetJob { get; private set; } = new();
        public TwoWayJob<TagsGetJob, ListResult<string>> TagsGetJob { get; private set; } = new();
        public TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult> FileDeepInfoGetJob { get; private set; } = new();
        public TwoWayJob<PipeServerJob, ValueResult<string>> PipeServerJob { get; private set; } = new();

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
                this.directoryHistoryaddJob?.Dispose();
                this.bookmarkDeleteJob?.Dispose();
                this.directoryViewCounterDeleteJob?.Dispose();
                this.fileRatingUpdateJob?.Dispose();
                this.fileTagDeleteJob?.Dispose();
                this.fileTagAddJob?.Dispose();
                this.gcCollectRunJob?.Dispose();

                this.ImageFileReadJob?.Dispose();
                this.ImageFileLoadingJob?.Dispose();
                this.ImageFileCacheJob?.Dispose();
                this.ThumbnailsGetJob?.Dispose();
                this.SubDirectoriesGetJob?.Dispose();
                this.DirectoryViewHistoryGetJob?.Dispose();
                this.AddressInfoGetJob?.Dispose();
                this.TagsGetJob?.Dispose();
                this.FileDeepInfoGetJob?.Dispose();
                this.PipeServerJob?.Dispose();
            }

            this.disposed = true;
        }

        public void StartBookmarkAddJob(Control sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (this.addBookmarkJob != null)
            {
                this.addBookmarkJob.WaitJobComplete();
                this.addBookmarkJob.Dispose();
            }

            this.addBookmarkJob = new();
            this.addBookmarkJob.SetCurrentSender(sender)
                .StartJob(sender, parameter);
        }

        public void StartSingleFileExportJob(Control sender, SingleFileExportParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (this.singleFileExportJob != null)
            {
                this.singleFileExportJob.WaitJobComplete();
                this.singleFileExportJob.Dispose();
            }

            this.singleFileExportJob = new();
            this.singleFileExportJob.SetCurrentSender(sender)
                .StartJob(sender, parameter);
        }

        public void StartDirectoryStateUpdateJob(Control sender, DirectoryStateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (this.directoryStateUpdateJob != null)
            {
                this.directoryStateUpdateJob.WaitJobComplete();
                this.directoryStateUpdateJob.Dispose();
            }

            this.directoryStateUpdateJob = new();
            this.directoryStateUpdateJob.SetCurrentSender(sender)
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewHistoryAddJob(Control sender, ValueParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (this.directoryHistoryaddJob != null)
            {
                this.directoryHistoryaddJob.WaitJobComplete();
                this.directoryHistoryaddJob.Dispose();
            }

            this.directoryHistoryaddJob = new();
            this.directoryHistoryaddJob.SetCurrentSender(sender)
                .StartJob(sender, parameter);
        }

        public void StartBookmarkDeleteJob(Control sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (this.bookmarkDeleteJob != null)
            {
                this.bookmarkDeleteJob.WaitJobComplete();
                this.bookmarkDeleteJob.Dispose();
            }

            this.bookmarkDeleteJob = new();
            this.bookmarkDeleteJob.SetCurrentSender(sender)
                .StartJob(sender, parameter);
        }

        public void StartDirectoryViewCounterDeleteJob(Control sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (this.directoryViewCounterDeleteJob != null)
            {
                this.directoryViewCounterDeleteJob.WaitJobComplete();
                this.directoryViewCounterDeleteJob.Dispose();
            }

            this.directoryViewCounterDeleteJob = new();
            this.directoryViewCounterDeleteJob.SetCurrentSender(sender)
                .StartJob(sender, parameter);
        }

        public void StartFileRatingUpdateJob(Control sender, FileRatingUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (this.fileRatingUpdateJob != null)
            {
                this.fileRatingUpdateJob.WaitJobComplete();
                this.fileRatingUpdateJob.Dispose();
            }

            this.fileRatingUpdateJob = new();
            this.fileRatingUpdateJob.SetCurrentSender(sender)
                .StartJob(sender, parameter);
        }

        public void StartFileTagDeleteJob(Control sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (this.fileTagDeleteJob != null)
            {
                this.fileTagDeleteJob.WaitJobComplete();
                this.fileTagDeleteJob.Dispose();
            }

            this.fileTagDeleteJob = new();
            this.fileTagDeleteJob.SetCurrentSender(sender)
                .StartJob(sender, parameter);
        }

        public void StartFileTagAddJob(Control sender, FileTagUpdateParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (this.fileTagAddJob != null)
            {
                this.fileTagAddJob.WaitJobComplete();
                this.fileTagAddJob.Dispose();
            }

            this.fileTagAddJob = new();
            this.fileTagAddJob.SetCurrentSender(sender)
                .StartJob(sender, parameter);
        }

        public void StartGCCollectRunJob(Control sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            if (this.gcCollectRunJob != null)
            {
                this.gcCollectRunJob.WaitJobComplete();
                this.gcCollectRunJob.Dispose();
            }

            this.gcCollectRunJob = new();
            this.gcCollectRunJob.SetCurrentSender(sender)
                .StartJob(sender);
        }
    }
}
