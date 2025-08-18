using PicSum.Job.Entities;
using SWF.Core.Job;

namespace PicSum.Job.Results
{

    public sealed class ImageFileReadResult
        : IJobResult
    {
        public int Index { get; internal set; } = 0;
        public ImageFileEntity Image { get; internal set; } = ImageFileEntity.EMPTY;
        public bool IsMain { get; internal set; }
        public bool HasSub { get; internal set; }
    }
}
