using PicSum.Job.Entities;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    public sealed class ImageFileReadResult
        : IJobResult
    {
        public ImageFileEntity? Image { get; internal set; }
        public bool IsMain { get; internal set; }
        public bool HasSub { get; internal set; }
    }
}
