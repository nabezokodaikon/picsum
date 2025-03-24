using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class FilesGetByTagParameter
        : IJobParameter
    {
        public string Tag { get; set; } = string.Empty;
        public bool IsGetThumbnail { get; set; }
    }
}
