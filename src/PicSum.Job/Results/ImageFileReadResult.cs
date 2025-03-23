using PicSum.Job.Entities;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Results
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public struct ImageFileReadResult
        : IJobResult, IEquatable<ImageFileReadResult>
    {
        public ImageFileEntity Image { get; internal set; }
        public bool IsMain { get; internal set; }
        public bool HasSub { get; internal set; }

        public readonly bool Equals(ImageFileReadResult other)
        {
            if (!this.Image.Equals(other.Image)) { return false; }
            if (this.IsMain != other.IsMain) { return false; }
            if (this.HasSub != other.HasSub) { return false; }

            return true;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(ImageFileReadResult))
            {
                return false;
            }

            return this.Equals((ImageFileReadResult)obj);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.Image, this.IsMain, this.HasSub);
        }

        public static bool operator ==(ImageFileReadResult left, ImageFileReadResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ImageFileReadResult left, ImageFileReadResult right)
        {
            return !(left == right);
        }
    }
}
