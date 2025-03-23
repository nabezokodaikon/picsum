using PicSum.Job.Entities;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    public struct ImageFileReadResult
        : IJobResult, IEquatable<ImageFileReadResult>
    {
        public ImageFileEntity? Image { get; internal set; }
        public bool IsMain { get; internal set; }
        public bool HasSub { get; internal set; }

        public bool Equals(ImageFileReadResult other)
        {
            if (this.Image != other.Image) { return false; }
            if (this.IsMain != other.IsMain) { return false; }
            if (this.HasSub != other.HasSub) { return false; }

            return true;
        }
    }
}
