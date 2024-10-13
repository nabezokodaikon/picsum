using SWF.Core.Job;
using PicSum.Job.Entities;

namespace PicSum.Job.Results
{
    public class ImageFileReadResult
        : IJobResult
    {
        public ImageFileEntity? Image { get; internal set; }
        public bool IsMain { get; internal set; }
        public bool HasSub { get; internal set; }
    }
}
