namespace PicSum.Core.Task.AsyncTaskV2
{
    public abstract class AbstractAsyncTask<TParameter>
        : AbstractAsyncTask<TParameter, TaskEmptyResult>,
          IAsyncTask
        where TParameter : AbstractTaskParameter
    {

    }

    public abstract class AbstractAsyncTask<TParameter, TResult>
        : IAsyncTask
        where TParameter : AbstractTaskParameter
        where TResult : AbstractTaskResult
    {
        private long isCancel = 0;

        public TaskID? ID { get; internal set; }
        internal TParameter? Parameter { get; set; }
        internal Action? WaitAction;
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

        public AbstractAsyncTask()
        {

        }

        internal void ExecuteWrapper()
        {
            if (this.Parameter == null)
                throw new NullReferenceException("タスクパラーターがNULLです。");

            try
            {
                this.Execute(this.Parameter);
            }
            catch (TaskException ex)
            {
                if (this.CatchAction == null)
                    throw new NullReferenceException("タスク例外アクションがNULLです。");
                this.CatchAction(ex);
            }
            catch (TaskCancelException)
            {
                throw;
            }
            finally
            {
                if (this.CompleteAction == null)
                    throw new NullReferenceException("タスク完了アクションがNULLです。");
                this.CompleteAction();
            }
        }

        protected abstract void Execute(TParameter parameter);

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

        protected void Wait()
        {
            if (this.WaitAction == null)
                throw new NullReferenceException("タスク待機アクションがNULLです。");

            this.WaitAction();
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

        protected void Complete()
        {
            if (this.CompleteAction != null)
            {
                this.CompleteAction();
            }
        }
    }
}
