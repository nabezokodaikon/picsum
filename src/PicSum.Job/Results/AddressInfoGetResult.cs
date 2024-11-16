using PicSum.Job.Entities;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    public sealed class AddressInfoGetResult
        : IJobResult
    {
        public string? DirectoryPath { get; internal set; }
        public List<FileShallowInfoEntity>? DirectoryList { get; internal set; }
        public bool HasSubDirectory { get; internal set; }
    }
}
