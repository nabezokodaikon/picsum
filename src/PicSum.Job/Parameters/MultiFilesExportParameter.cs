using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Parameters
{
    public struct MultiFilesExportParameter
        : IJobParameter
    {
        public string[]? SrcFiles { get; set; }
        public string? ExportDirecotry { get; set; }
    }
}
