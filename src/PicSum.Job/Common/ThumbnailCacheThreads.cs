using NLog;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace PicSum.Job.Common
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailCacheThreads
        : IThumbnailCacheThreads
    {
        private static readonly Logger logger
            = LogManager.GetCurrentClassLogger();

        private const int THREAD_COUNT = 4;

        private bool disposed = false;
        private readonly ConcurrentQueue<ThumbnailReadThreadsEntity>[] queueArray
            = new ConcurrentQueue<ThumbnailReadThreadsEntity>[THREAD_COUNT];
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

        public ThumbnailCacheThreads()
        {
            for (var i = 0; i < this.threads.Length; i++)
            {
                var index = i;
                this.queueArray[index]
                    = new ConcurrentQueue<ThumbnailReadThreadsEntity>();
                this.threads[index] = Task.Run(() => this.DoWork(index));
            }
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
                this.Clear();
                this.IsAbort = true;
                Task.WaitAll(this.threads);
            }

            this.disposed = true;
        }

        private void Clear()
        {
            foreach (var queue in this.queueArray)
            {
                while (queue.TryDequeue(out var _)) { }
            }
        }

        public void DoCache(
            ThumbnailsGetParameter parameter,
            Action<ThumbnailImageResult> callbackAction)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
            ArgumentNullException.ThrowIfNull(callbackAction, nameof(callbackAction));

            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            this.Clear();

            var array = parameter.FilePathList
                .Skip(parameter.FirstIndex)
                .Take(parameter.LastIndex - parameter.FirstIndex + 1)
                .Select(_ => new ThumbnailReadThreadsEntity(
                    _,
                    parameter.ThumbnailWidth,
                    parameter.ThumbnailHeight,
                    callbackAction))
                .ToArray();

            for (var i = 0; i < array.Length; i++)
            {
                var queueIndex = i % THREAD_COUNT;
                this.queueArray[queueIndex].Enqueue(array[i]);
            }
        }

        private void DoWork(int index)
        {
            Thread.CurrentThread.Name = $"ThumbnailReadThreads: [{index}]";

            logger.Debug("サムネイル読み込みスレッドが開始されました。");

            try
            {
                var queue = this.queueArray[index];

                while (true)
                {
                    if (this.IsAbort)
                    {
                        logger.Debug("サムネイル読み込みスレッドに中断リクエストがありました。");
                        return;
                    }

                    if (queue.TryDequeue(out var entity))
                    {
                        try
                        {
                            var bf = Instance<IThumbnailCacher>.Value.GetOrCreateCache(
                                entity.FilePath, entity.ThumbnailWidth, entity.ThumbnailHeight);
                            if (bf != ThumbnailCacheEntity.EMPTY
                                && bf.ThumbnailBuffer != null)
                            {
                                var img = new ThumbnailImageResult
                                {
                                    FilePath = bf.FilePath,
                                    ThumbnailImage = ThumbnailUtil.ToImage(bf.ThumbnailBuffer),
                                    ThumbnailWidth = bf.ThumbnailWidth,
                                    ThumbnailHeight = bf.ThumbnailHeight,
                                    SourceWidth = bf.SourceWidth,
                                    SourceHeight = bf.SourceHeight,
                                    FileUpdatedate = bf.FileUpdatedate
                                };

                                entity.CallbackAction(img);
                            }
                        }
                        catch (FileUtilException ex)
                        {
                            logger.Error(ex);
                        }
                        catch (ImageUtilException ex)
                        {
                            logger.Error(ex);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, $"サムネイル読み込みスレッドで補足されない例外が発生しました。");
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            }
            finally
            {
                logger.Debug("サムネイル読み込みスレッドが終了します。");
            }
        }
    }
}
