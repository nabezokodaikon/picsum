using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class TakenDatesGetParameter
        : IJobParameter
    {
        public string[] FilePathList { get; set; } = [];
    }
}
