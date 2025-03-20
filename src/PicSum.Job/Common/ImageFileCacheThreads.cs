using NLog;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System.Collections.Concurrent;

namespace PicSum.Job.Common
{
    internal sealed class ImageFileCacheThreads
        : IImageFileCacheThreads
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const int THREAD_COUNT = 4;

        private bool disposed = false;
        private readonly ConcurrentQueue<string>[] queueArray
            = new ConcurrentQueue<string>[THREAD_COUNT];
        private readonly Task[] threads = new Task[THREAD_COUNT];
        private long isAbort = 0;

        private bool IsAbort
        {
            get
            {
                return Interlocked.Read(ref this.isAbort) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this.isAbort, Convert.ToInt64(value));
            }
        }

        public ImageFileCacheThreads()
        {
            for (var i = 0; i < this.threads.Length; i++)
            {
                var index = i;
                this.queueArray[index] = new ConcurrentQueue<string>();
                this.threads[index] = Task.Run(() => this.DoWork(index));
            }
        }

        ~ImageFileCacheThreads()
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
                foreach (var queue in this.queueArray)
                {
                    while (queue.TryDequeue(out var _)) { }
                }

                this.IsAbort = true;
                Task.WaitAll(this.threads);
            }

            this.disposed = true;
        }

        public void DoCache(string[] files)
        {
            foreach (var queue in this.queueArray)
            {
                while (queue.TryDequeue(out var _)) { }
            }

            for (var i = 0; i < files.Length; i += THREAD_COUNT)
            {
                for (var j = 0; j < THREAD_COUNT; j++)
                {
                    var k = i + j;
                    if (k >= files.Length)
                    {
                        break;
                    }

                    var queue = this.queueArray[j];
                    queue.Enqueue(files[k]);
                }
            }
        }

        private void DoWork(int index)
        {
            Thread.CurrentThread.Name = $"ImageFileCacheThreads: [{index}]";

            Logger.Debug("画像ファイルキャッシュスレッドが開始されました。");

            try
            {
                var queue = this.queueArray[index];

                while (true)
                {
                    if (this.IsAbort)
                    {
                        Logger.Debug("画像ファイルキャッシュスレッドに中断リクエストがありました。");
                        return;
                    }

                    if (queue.TryDequeue(out var file))
                    {
                        try
                        {
                            Instance<IImageFileCacher>.Value.Create(file);
                            var size = Instance<IImageFileCacher>.Value.GetSize(file);
                            Instance<IImageFileSizeCacher>.Value.Set(file, size);
                        }
                        catch (FileUtilException ex)
                        {
                            Logger.Error(ex);
                        }
                        catch (ImageUtilException ex)
                        {
                            Logger.Error(ex);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, $"画像ファイルキャッシュスレッドで補足されない例外が発生しました。");
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            finally
            {
                Logger.Debug("画像ファイルキャッシュスレッドが終了します。");
            }
        }
    }
}
