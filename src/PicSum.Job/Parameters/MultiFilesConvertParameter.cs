using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class MultiFilesConvertParameter
        : IJobParameter
    {
        public string[]? SrcFiles { get; set; }
        public string? ExportDirecotry { get; set; }
        public FileUtil.ImageFileFormat ImageFileFormat { get; set; }
    }
}
