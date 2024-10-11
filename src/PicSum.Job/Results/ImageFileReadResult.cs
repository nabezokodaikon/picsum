using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;

namespace PicSum.Job.Results
{
    public class ImageFileReadResult
        : IJobResult
    {
        public ImageFileEntity? Image { get; set; }
        public bool IsMain { get; set; }
        public bool HasSub { get; set; }
    }
}
