using SWF.Core.ConsoleAccessor;
using System.Diagnostics;

namespace SWF.Core.Job
{
    public abstract class AbstractAsyncJob
        : IAsyncJob
    {
        private long _isCancel = 0;

        internal ISender? Sender { get; set; } = null;

        public JobID ID { get; private set; } = JobID.GetNew();

        private bool IsCancel
        {
            get
            {
                return Interlocked.Read(ref this._isCancel) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this._isCancel, Convert.ToInt64(value));
            }
        }

        public void CheckCancel()
        {
            if (this.IsCancel)
            {
                throw new JobCancelException(this.ID);
            }
        }

        public void WriteErrorLog(JobException ex)
        {
            Log.GetLogger().Error($"{this.ID} {ex}");
        }

        internal abstract Task ExecuteWrapper();

        internal void BeginCancel()
        {
            this.IsCancel = true;
        }

        internal bool CanUIThreadAccess()
        {
            if (this.Sender == null)
            {
                throw new InvalidOperationException("呼び出し元のコントロールが設定されていません。");
            }

            if (this.Sender.IsHandleCreated
                && !this.Sender.IsDisposed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public abstract class AbstractTwoWayJob<TParameter, TResult>
        : AbstractAsyncJob
        where TParameter : class, IJobParameter
        where TResult : IJobResult
    {
        internal TParameter? Parameter { get; set; } = null;
        internal Action<TResult>? CallbackAction { get; set; } = null;

        public AbstractTwoWayJob()
        {

        }

        internal override async Task ExecuteWrapper()
        {
            var jobName = $"{this.GetType().Name} {this.ID}";
            var logger = Log.GetLogger();

            using (TimeMeasuring.Run(false, jobName))
            {
                logger.Trace($"{jobName} を実行します。");
                var sw = Stopwatch.StartNew();
                try
                {
                    if (this.Parameter != null)
                    {
                        await this.Execute(this.Parameter);
                    }
                    else
                    {
                        await this.Execute();
                    }
                }
                catch (JobCancelException)
                {
                    logger.Trace($"{jobName} がキャンセルされました。");
                }
                catch (JobException ex)
                {
                    logger.Error($"{jobName} {ex}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{jobName} で補足されない例外が発生しました。");
                }
                finally
                {
                    sw.Stop();
                    logger.Trace($"{jobName} が終了しました。{sw.ElapsedMilliseconds} ms");
                }
            }
        }

        protected virtual Task Execute(TParameter parameter)
        {
            throw new NotImplementedException();
        }

        protected virtual Task Execute()
        {
            throw new NotImplementedException();
        }

        protected void Callback(TResult result)
        {
            ArgumentNullException.ThrowIfNull(result, nameof(result));

            this.CallbackAction?.Invoke(result);
        }
    }

    public abstract class AbstractTwoWayJob<TResult>
        : AbstractTwoWayJob<EmptyParameter, TResult>
        where TResult : IJobResult
    {

    }

    public abstract class AbstractOneWayJob<TParameter>
        : AbstractTwoWayJob<TParameter, EmptyResult>
        where TParameter : class, IJobParameter
    {

    }

    public abstract class AbstractOneWayJob
        : AbstractTwoWayJob<EmptyParameter, EmptyResult>
    {

    }
}
