using NLog;

namespace PicSum.Core.Job.AsyncJob
{
    public abstract class AbstractTwoWayJob<TParameter, TResult>
        : IAsyncJob
        where TParameter : IJobParameter
        where TResult : IJobResult
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private long isCancel = 0;

        public JobID? ID { get; set; }
        internal TParameter? Parameter { get; set; }
        internal Action<TResult>? CallbackAction { get; set; }
        internal Action<JobException>? CatchAction { get; set; }
        internal Action? CompleteAction { get; set; }

        private bool IsCancel
        {
            get
            {
                return Interlocked.Read(ref this.isCancel) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this.isCancel, Convert.ToInt64(value));
            }
        }

        public AbstractTwoWayJob()
        {

        }

        public void WriteErrorLog(JobException ex)
        {
            Logger.Error($"{this.ID} {ex}");
        }

        internal void ExecuteWrapper()
        {
            try
            {
                if (this.Parameter != null)
                {
                    this.Execute(this.Parameter);
                }
                else
                {
                    this.Execute();
                }

                this.CompleteAction?.Invoke();
            }
            catch (JobCancelException)
            {
                throw;
            }
            catch (JobException ex)
            {
                Logger.Error($"{this.ID} {ex}");
                this.CatchAction?.Invoke(ex);
            }
        }

        protected virtual void Execute(TParameter parameter)
        {
            throw new NotImplementedException();
        }

        protected virtual void Execute()
        {
            throw new NotImplementedException();
        }

        internal void BeginCancel()
        {
            this.IsCancel = true;
        }

        public void CheckCancel()
        {
            if (this.IsCancel)
            {
                if (this.ID == null)
                {
                    throw new NullReferenceException("ジョブIDがNULLです。");
                }

                throw new JobCancelException(this.ID);
            }
        }

        protected void Callback(TResult result)
        {
            ArgumentNullException.ThrowIfNull(result, nameof(result));

            this.CallbackAction?.Invoke(result);
        }
    }

    public abstract class AbstractTwoWayJob<TResult>
        : AbstractTwoWayJob<EmptyParameter, TResult>,
          IAsyncJob
        where TResult : IJobResult
    {

    }

    public abstract class AbstractOneWayJob<TParameter>
        : AbstractTwoWayJob<TParameter, EmptyResult>,
          IAsyncJob
        where TParameter : IJobParameter
    {

    }

    public abstract class AbstractOneWayJob
        : AbstractTwoWayJob<EmptyParameter, EmptyResult>,
          IAsyncJob
    {

    }
}
