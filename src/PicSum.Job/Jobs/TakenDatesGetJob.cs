using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class TakenDatesGetJob
        : AbstractTwoWayJob<TakenDatesGetParameter, TakenDateResult>
    {
        private long _hasTakenDates = 0;

        private bool HasTakenDates
        {
            get
            {
                return Interlocked.Read(ref this._hasTakenDates) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this._hasTakenDates, Convert.ToInt64(value));
            }
        }

        protected override ValueTask Execute(TakenDatesGetParameter param)
        {
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    Parallel.For(
                        0,
                        param.FilePathList.Length,
                        new ParallelOptions
                        {
                            CancellationToken = cts.Token,
                            MaxDegreeOfParallelism = Math.Max(param.FilePathList.Length, 1),
                        },
                        index =>
                    {
                        if (this.IsJobCancel)
                        {
                            cts.Cancel();
                            cts.Token.ThrowIfCancellationRequested();
                        }

                        try
                        {
                            var filePath = param.FilePathList[index];
                            var takenDate = Instance<IImageFileTakenCacher>.Value.GetOrCreate(filePath);
                            if (takenDate == FileUtil.EMPTY_DATETIME)
                            {
                                return;
                            }

                            this.HasTakenDates = true;
                            this.Callback(new TakenDateResult(filePath, takenDate));
                        }
                        catch (ImageUtilException ex)
                        {
                            this.WriteErrorLog(ex);
                        }
                    });
                }
                catch (OperationCanceledException)
                {
                    return ValueTask.CompletedTask;
                }
            }

            if (this.HasTakenDates)
            {
                this.Callback(TakenDateResult.COMPLETED);
            }

            return ValueTask.CompletedTask;
        }
    }
}
