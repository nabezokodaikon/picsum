using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public struct SingleFileExportParameter
        : IJobParameter
    {
        public string? SrcFilePath { get; set; }
        public string? ExportFilePath { get; set; }
    }
}
