using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public struct MultiFilesExportParameter
        : IJobParameter
    {
        public string[]? SrcFiles { get; set; }
        public string? ExportDirecotry { get; set; }
    }
}
