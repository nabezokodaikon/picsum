namespace SWF.Core.Job
{
    internal interface IThreadWrapper
        : IDisposable
    {
        public void Start(Action dowork);
        public void Wait(Action dowork);
    }

    internal sealed partial class JobThread
        : IThreadWrapper
    {
        private bool disposed = false;
        private Thread? thread = null;
        private Action? dowork;

        public JobThread()
        {

        }

        ~JobThread()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.thread = null;
            }

            this.disposed = true;
        }

        public void Start(Action dowork)
        {
            ArgumentNullException.ThrowIfNull(dowork, nameof(dowork));

            if (this.thread == null)
            {
                this.thread = new(dowork.Invoke);
                this.thread.Priority = ThreadPriority.Highest;
                this.thread.Start();
            }
        }

        public void Wait(Action dowork)
        {
            this.thread?.Join();
        }
    }

    internal sealed partial class JobTask
        : IThreadWrapper
    {
        private bool disposed = false;
        private Task? task = null;

        public JobTask()
        {

        }

        ~JobTask()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.task?.Dispose();
                this.task = null;
            }

            this.disposed = true;
        }

        public void Start(Action dowork)
        {
            ArgumentNullException.ThrowIfNull(dowork, nameof(dowork));

            this.task ??= Task.Run(dowork);
        }

        public void Wait(Action dowork)
        {
            this.task?.Wait();
        }
    }

}
