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
        public TParameter? Parameter { get; internal set; }
        public Action<TResult>? ThenAction { get; internal set; }
        public Action<Exception>? CatchAction { get; internal set; }
        public Action? CompleteAction { get; internal set; }

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
            try
            {
                if (this.Parameter == null)
                    throw new NullReferenceException(nameof(this.Parameter));

                this.Execute(this.Parameter);
            }
            catch (TaskException ex)
            {
                this.CatchAction?.Invoke(ex);
            }
            catch (TaskCancelException)
            {
                throw;
            }
            finally
            {
                GC.Collect();
            }

            this.CompleteAction?.Invoke();
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
                if (this.ID != null)
                {
                    throw new TaskCancelException(this.ID);
                }
                else
                {
                    throw new TaskCancelException();
                }
            }
        }
    }
}
