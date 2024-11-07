using PicSum.Job.Entities;

namespace PicSum.Job.Common
{
    public interface IClipFiles
        : IDisposable
    {
        public void AddFiles(IList<string> files);
        public void RemoveFiles(IList<string> files);
        public IList<ClipFileEntity> GetFiles();
    }
}
