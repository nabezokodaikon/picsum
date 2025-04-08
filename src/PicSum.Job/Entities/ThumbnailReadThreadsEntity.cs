using PicSum.Job.Results;

namespace PicSum.Job.Entities
{
    internal sealed class ThumbnailReadThreadsEntity
    {
        public string FilePath { get; private set; }
        public int ThumbnailWidth { get; private set; }
        public int ThumbnailHeight { get; private set; }
        public Action<ThumbnailImageResult> CallbackAction { get; private set; }

        public ThumbnailReadThreadsEntity(
            string filePath,
            int thumbnailWidth,
            int thumbnailHeight,
            Action<ThumbnailImageResult> callbackAction)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(callbackAction, nameof(callbackAction));

            this.FilePath = filePath;
            this.ThumbnailWidth = thumbnailWidth;
            this.ThumbnailHeight = thumbnailHeight;
            this.CallbackAction = callbackAction;
        }
    }
}
