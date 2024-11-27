using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class SingleFileConvertParameter
        : IJobParameter
    {
        public string? SrcFilePath { get; set; }
        public string? ExportFilePath { get; set; }
        public FileUtil.ImageFileFormat ImageFileFormat { get; set; }
    }
}
