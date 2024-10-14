using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class SingleFileExportParameter
        : IJobParameter
    {
        public string? SrcFilePath { get; set; }
        public string? ExportFilePath { get; set; }
    }
}
