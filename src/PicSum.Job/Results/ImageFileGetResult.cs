using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;

namespace PicSum.Job.Results
{
    public class ImageFileGetResult
        : IJobResult
    {
        public ImageFileEntity? Image { get; set; }
        public bool IsMain { get; set; }
        public bool HasSub { get; set; }
    }
}
