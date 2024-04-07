using NLog;
using SWF.Common;

namespace PicSum.Core.Task.AsyncTaskV2
{
    public abstract class AbstractTwoWayTask<TParameter, TResult>
        : IAsyncTask
        where TParameter : ITaskParameter
        where TResult : ITaskResult
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private long isCancel = 0;

        public TaskID? ID { get; internal set; }
        internal TParameter? Parameter { get; set; }
        internal Action<TResult>? CallbackAction { get; set; }
        internal Action<TaskException>? CatchAction { get; set; }
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

        public AbstractTwoWayTask()
        {

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

                if (this.CompleteAction != null)
                {
                    this.CompleteAction();
                }
            }
            catch (TaskCancelException)
            {
                throw;
            }
            catch (TaskException ex)
            {
                Logger.Error($"{this.ID} {ex}");

                if (this.CatchAction != null)
                {
                    this.CatchAction(ex);
                }
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

        protected void WriteErrorLog(Exception ex)
        {
            Logger.Error($"{this.ID} {ex}");
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
                    throw new NullReferenceException("タスクIDがNULLです。");
                throw new TaskCancelException(this.ID);
            }
        }

        protected void Callback(TResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (this.CallbackAction != null)
            {
                this.CallbackAction(result);
            }
        }
    }

    public abstract class AbstractTwoWayTask<TResult>
        : AbstractTwoWayTask<EmptyParameter, TResult>,
          IAsyncTask
        where TResult : ITaskResult
    {

    }

    public abstract class AbstractOneWayTask<TParameter>
        : AbstractTwoWayTask<TParameter, EmptyResult>,
          IAsyncTask
        where TParameter : ITaskParameter
    {

    }

    public abstract class AbstractOneWayTask
        : AbstractTwoWayTask<EmptyParameter, EmptyResult>,
          IAsyncTask
    {

    }
}
