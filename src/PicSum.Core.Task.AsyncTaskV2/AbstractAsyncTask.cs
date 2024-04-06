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

        public TaskID? ID { get; set; }
        public TParameter? Parameter { get; set; }
        public Action<TResult>? ThenAction { get; set; }
        public Action<Exception>? CatchAction { get; set; }
        public Action? CompleteAction { get; set; }

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

        public void ExecuteWrapper()
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

            this.CompleteAction?.Invoke();
        }

        protected abstract void Execute(TParameter parameter);

        public void BeginCancel()
        {
            this.IsCancel = true;
        }

        public void CheckCancel()
        {
            if (this.IsCancel)
            {
                throw new TaskCancelException();
            }
        }
    }
}
