using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// スタートアップパラメータエンティティ
    /// </summary>
    public sealed class StartupPrameter
        : IJobParameter
    {
        public string FileInfoDBFilePath { get; private set; }
        public string ThumbnailDBFilePath { get; private set; }

        public StartupPrameter(string fileInfoDBFilePath, string thumbnailDBFilePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(fileInfoDBFilePath, nameof(fileInfoDBFilePath));
            ArgumentNullException.ThrowIfNullOrEmpty(thumbnailDBFilePath, nameof(thumbnailDBFilePath));

            this.FileInfoDBFilePath = fileInfoDBFilePath;
            this.ThumbnailDBFilePath = thumbnailDBFilePath;
        }
    }
}
