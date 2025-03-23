using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public struct FilesGetByTagParameter
        : IJobParameter
    {
        public string Tag { get; set; }
        public bool IsGetThumbnail { get; set; }
    }
}
