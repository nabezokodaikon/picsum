using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class ImageFileGetByDirectoryParameter
        : IJobParameter
    {
        public string FilePath { get; private set; }

        public ImageFileGetByDirectoryParameter(string filePath)
        {
            this.FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }
}
