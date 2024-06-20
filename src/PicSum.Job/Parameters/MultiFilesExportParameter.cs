using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Paramters
{
    public sealed class MultiFilesExportParameter
        : IJobParameter
    {
        public string[]? SrcFiles { get; set; }
        public string? ExportDirecotry { get; set; }
    }
}
