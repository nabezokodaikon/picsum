using NLog;

namespace SWF.Core.Job
{
    public abstract class AbstractAsyncJob
        : IAsyncJob
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private long isCancel = 0;

        internal ISender? Sender { get; set; } = null;

        public JobID ID { get; private set; } = JobID.GetNew();

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

        internal abstract void ExecuteWrapper();

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
        where TParameter : IJobParameter
        where TResult : IJobResult
    {
        internal TParameter? Parameter { get; set; } = default;
        internal Action<TResult>? CallbackAction { get; set; } = null;

        public AbstractTwoWayJob()
        {

        }

        internal override void ExecuteWrapper()
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
