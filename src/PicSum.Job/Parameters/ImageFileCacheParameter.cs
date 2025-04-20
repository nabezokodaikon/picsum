using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class ImageFileCacheParameter
        : IJobParameter
    {
        public int CurrentIndex { get; private set; }
        public int NextCount { get; private set; }
        public int PreviewCount { get; private set; }
        public string[] Files { get; private set; }

        public ImageFileCacheParameter(
            int currentIndex, int nextCount, int previewCount, string[] files)
        {
            ArgumentNullException.ThrowIfNull(files, nameof(files));

            this.CurrentIndex = currentIndex;
            this.NextCount = nextCount;
            this.PreviewCount = previewCount;
            this.Files = files;
        }
    }
}
