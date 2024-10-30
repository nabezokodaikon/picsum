using NLog;

namespace SWF.Core.Job
{
    public abstract class AbstractAsyncJob
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private long isCancel = 0;
        private long isCompleted = 0;

#pragma warning disable CS8618
        internal ISender Sender { get; set; }
#pragma warning restore CS8618

        public JobID ID { get; private set; } = JobID.GetNew();

        internal bool IsCompleted
        {
            get
            {
                return Interlocked.Read(ref this.isCompleted) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this.isCompleted, Convert.ToInt64(value));
            }
        }

        public bool IsCancel
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

        public void CheckCancel()
        {
            if (this.IsCancel)
            {
                throw new JobCancelException(this.ID);
            }
        }

        public void WriteErrorLog(JobException ex)
        {
            Logger.Error($"{this.ID} {ex}");
        }

        internal void BeginCancel()
        {
            this.IsCancel = true;
        }
    }

    public abstract class AbstractTwoWayJob<TParameter, TResult>
        : AbstractAsyncJob
        where TParameter : IJobParameter
        where TResult : IJobResult
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal TParameter? Parameter { get; set; }
        internal Action<TResult>? CallbackAction { get; set; } = null;
        internal Action? CancelAction { get; set; } = null;
        internal Action<JobException>? CatchAction { get; set; } = null;
        internal Action? CompleteAction { get; set; } = null;

        public AbstractTwoWayJob()
        {

        }

        internal void ExecuteWrapper()
        {
            if (this.Parameter != null && !this.Parameter.Equals(default(TParameter)))
            {
                this.Execute(this.Parameter);
            }
            else
            {
                this.Execute();
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
        where TParameter : IJobParameter
    {

    }

    public abstract class AbstractOneWayJob
        : AbstractTwoWayJob<EmptyParameter, EmptyResult>
    {

    }
}
