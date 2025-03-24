using PicSum.Job.Entities;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Results
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class AddressInfoGetResult
        : IJobResult
    {
        public string DirectoryPath { get; internal set; } = string.Empty;
        public List<FileShallowInfoEntity>? DirectoryList { get; internal set; }
        public bool HasSubDirectory { get; internal set; }
    }
}
