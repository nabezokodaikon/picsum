using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Results
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class TakenDateResult
        : IJobResult
    {
        public static readonly TakenDateResult COMPLETED = new()
        {
            FilePath = string.Empty,
            TakenDate = DateTimeExtensions.EMPTY,
            IsCompleted = true,
        };

        public string FilePath { get; internal set; } = string.Empty;
        public DateTime TakenDate { get; internal set; } = DateTimeExtensions.EMPTY;
        public bool IsCompleted { get; internal set; } = false;

        private TakenDateResult()
        {

        }

        public TakenDateResult(string filePath, DateTime takenDate)
        {
            this.FilePath = filePath;
            this.TakenDate = takenDate;
            this.IsCompleted = false;
        }
    }
}
