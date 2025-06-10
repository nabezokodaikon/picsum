using NLog;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System.Collections.Concurrent;
using System.Drawing;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.Job.Common
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailCacheThreads
        : IThumbnailCacheThreads
    {
        private static readonly Logger logger
            = LogManager.GetCurrentClassLogger();

        private const int THREAD_COUNT = 4;

        private bool _disposed = false;
        private readonly ConcurrentQueue<ThumbnailReadThreadsEntity> _queue = new();
        private readonly Task[] _threads = new Task[THREAD_COUNT];
        private long _isAbort = 0;

        private bool IsAbort
        {
            get
            {
                return Interlocked.Read(ref this._isAbort) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this._isAbort, Convert.ToInt64(value));
            }
        }

        public ThumbnailCacheThreads()
        {
            for (var i = 0; i < this._threads.Length; i++)
            {
                var index = i;
                this._threads[index]
                    = Task.Run(() => this.DoWork(index));
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Clear();
                this.IsAbort = true;
                Task.WaitAll(this._threads);
            }

            this._disposed = true;
        }

        private void Clear()
        {
            while (this._queue.TryDequeue(out var _)) { }
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

            var entities = parameter.FilePathList
                .AsValueEnumerable()
                .Skip(parameter.FirstIndex)
                .Take(parameter.LastIndex - parameter.FirstIndex + 1)
                .Select(filePath => new ThumbnailReadThreadsEntity(
                    filePath,
                    parameter.ThumbnailWidth,
                    parameter.ThumbnailHeight,
                    callbackAction));

            foreach (var entity in entities)
            {
                this._queue.Enqueue(entity);
            }
        }

        private void DoWork(int index)
        {
            Thread.CurrentThread.Name = $"ThumbnailCacheThreads: [{index}]";

            logger.Debug("サムネイル読み込みスレッドが開始されました。");

            try
            {
                while (true)
                {
                    if (this.IsAbort)
                    {
                        logger.Debug("サムネイル読み込みスレッドに中断リクエストがありました。");
                        return;
                    }

                    if (this._queue.TryDequeue(out var entity))
                    {
                        try
                        {
                            var bf = Instance<IThumbnailCacher>.Value.GetOrCreateCache(
                                entity.FilePath, entity.ThumbnailWidth, entity.ThumbnailHeight);
                            if (bf != ThumbnailCacheEntity.EMPTY
                                && bf.ThumbnailBuffer != null)
                            {
                                Instance<IImageFileSizeCacher>.Value.Set(
                                    bf.FilePath,
                                    new Size(bf.SourceWidth, bf.SourceHeight),
                                    bf.FileUpdatedate);

                                var img = new ThumbnailImageResult
                                {
                                    FilePath = bf.FilePath,
                                    ThumbnailImage = new CvImage(
                                        bf.FilePath,
                                        ThumbnailUtil.ToImage(bf.ThumbnailBuffer)),
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
                        Thread.Sleep(1);
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
