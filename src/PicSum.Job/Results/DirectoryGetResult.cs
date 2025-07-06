using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Results
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class DirectoryGetResult
        : IJobResult
    {
        public string DirectoryPath { get; internal set; } = string.Empty;
        public DirectoryStateParameter DirectoryState { get; internal set; } = DirectoryStateParameter.EMPTY;
        public FileShallowInfoEntity[]? FileInfoList { get; internal set; }
    }
}
