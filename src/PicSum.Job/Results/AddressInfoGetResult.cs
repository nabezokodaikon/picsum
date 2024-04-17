using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;

namespace PicSum.Job.Results
{
    public sealed class AddressInfoGetResult
        : IJobResult
    {
        public string? DirectoryPath { get; set; }
        public IList<FileShallowInfoEntity>? DirectoryList { get; set; }
        public bool HasSubDirectory { get; set; }
    }
}
