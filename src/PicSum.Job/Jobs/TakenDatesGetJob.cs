using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    public sealed class TakenDatesGetJob
        : AbstractTwoWayJob<TakenDatesGetParameter, TakenDateResult>
    {
        private const int MAX_DEGREE_OF_PARALLELISM = 4;

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

        protected async override ValueTask Execute(TakenDatesGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var files = param.FilePathList
                .AsEnumerable()
                .Where(static _ => ImageUtil.CanRetainExifImageFormat(_))
                .ToArray();
            if (files.Length < 1)
            {
                return;
            }

            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    await Parallel.ForAsync(
                        0,
                        files.Length,
                        new ParallelOptions
                        {
                            CancellationToken = cts.Token,
                            MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                        },
                        async (index, _) =>
                        {
                            if (this.IsJobCancel)
                            {
                                await cts.CancelAsync().False();
                                cts.Token.ThrowIfCancellationRequested();
                            }

                            try
                            {
                                var filePath = files[index];
                                var takenDate = await Instance<IImageFileTakenDateCacher>.Value.GetOrCreate(filePath).False();
                                if (takenDate.IsEmpty())
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
                        }).False();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            if (this.HasTakenDates)
            {
                this.Callback(TakenDateResult.COMPLETED);
            }
        }
    }
}
