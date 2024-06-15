using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;

namespace PicSum.Job.Results
{
    public class ImageFileGetResultV2
        : IJobResult
    {
        public ImageFileEntity? Image { get; set; }
        public bool IsMain { get; set; }
        public bool HasSub { get; set; }
    }
}
