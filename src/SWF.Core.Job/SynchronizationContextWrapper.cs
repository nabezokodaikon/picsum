using System.Collections.Concurrent;

namespace SWF.Core.Job
{
    public sealed partial class SynchronizationContextWrapper
        : IDisposable
    {
#pragma warning disable CS8618
        private static SynchronizationContextWrapper innerInstance;
#pragma warning restore CS8618

        public static SynchronizationContextWrapper Instance
        {
            get
            {
                if (innerInstance == null)
                {
                    throw new NullReferenceException("同期コンテキストがNullです。");
                }

                return innerInstance;
            }
        }

        public static void SetSynchronizationContext()
        {
            if (innerInstance != null)
            {
                throw new InvalidOperationException("同期コンテキストは既に設定されています。");
            }

            innerInstance = new SynchronizationContextWrapper();
        }

        public static void DisposeStaticResources()
        {
            innerInstance.Dispose();
        }

        private bool disposed = false;
        private readonly SynchronizationContext context;
        private readonly ConcurrentQueue<CallbackItem> callbackQueue;
        private readonly CancellationTokenSource source;
        private readonly Task thread;

        private SynchronizationContextWrapper()
        {
            if (SynchronizationContext.Current == null)
            {
                throw new NullReferenceException("同期コンテキストがNullです。");
            }

            this.context = SynchronizationContext.Current;
            this.callbackQueue = new();
            this.source = new();
            this.thread = Task.Run(() => this.DoWork(this.source.Token));
        }

        ~SynchronizationContextWrapper()
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
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.source.Cancel();
                    this.thread.Wait();

                    this.source.Dispose();
                    this.thread.Dispose();
                }

                this.disposed = true;
            }
        }

        private void DoWork(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (this.callbackQueue.TryDequeue(out var item))
                {
                    this.context.Post(_ =>
                    {
                        item.Callback(item.State);
                    }, null);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        public void Post(SendOrPostCallback d, object? state)
        {
            ArgumentNullException.ThrowIfNull(d, nameof(d));

            if (this.disposed)
            {
                throw new ObjectDisposedException("同期コンテキストは破棄されています。");
            }

            if (this.context == null)
            {
                throw new NullReferenceException("同期コンテキストがNullです。");
            }

            //this.context.Post(_ =>
            //{
            //    d(state);
            //}, null);

            this.callbackQueue.Enqueue(new CallbackItem(d, state));
        }
    }
}
