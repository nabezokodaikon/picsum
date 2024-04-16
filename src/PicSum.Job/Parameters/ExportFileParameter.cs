using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Paramters
{
    public sealed class ExportFileParameter
        : IJobParameter
    {
        public string? SrcFilePath { get; set; }
        public string? ExportFilePath { get; set; }
    }
}
