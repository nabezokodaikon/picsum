using NLog;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract class AbstractAsyncJob
        : IAsyncJob
    {
        protected static readonly Logger LOGGER = Log.GetLogger();

        private readonly JobID _id;
        private readonly string _name;
        private long _isCancel = 0;

        public bool IsJobCancel
        {
            get
            {
                return Interlocked.Read(ref this._isCancel) == 1;
            }
            private set
            {
                Interlocked.Exchange(ref this._isCancel, Convert.ToInt64(value));
            }
        }

        internal ISender? Sender { get; set; } = null;

        public AbstractAsyncJob()
        {
            this._id = JobID.GetNew();
            this._name = $"{this.GetType().Name} {this._id}";
        }

        public void ThrowIfJobCancellationRequested()
        {
            if (this.IsJobCancel)
            {
                throw new JobCancelException(this._name);
            }
        }

        public void WriteErrorLog(Exception ex)
        {
            ArgumentNullException.ThrowIfNull(ex, nameof(ex));

            LOGGER.Error(ex, $"{this} で例外が発生しました。");
        }

        public void WriteErrorLog(string message)
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));

            LOGGER.Error($"{this} で例外が発生しました。{message}");
        }

        public override string ToString()
        {
            return this._name;
        }

        internal abstract Task ExecuteWrapper(CancellationToken token);

        internal void BeginCancel()
        {
            this.IsJobCancel = true;
        }

        internal bool CanUIThreadAccess()
        {
            if (Thread.CurrentThread.Name != AppConstants.UI_THREAD_NAME)
            {
                throw new InvalidOperationException("現在のスレッドがUIスレッドではありません。");
            }

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

    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract class AbstractTwoWayJob<TParameter, TResult>
        : AbstractAsyncJob
        where TParameter : class, IJobParameter
        where TResult : IJobResult
    {
        public CancellationToken CancellationToken { get; private set; } = CancellationToken.None;

        internal TParameter? Parameter { get; set; } = null;
        internal Action<TResult>? CallbackAction { get; set; } = null;

        public AbstractTwoWayJob()
        {

        }

        internal override async Task ExecuteWrapper(CancellationToken token)
        {
            this.CancellationToken = token;

            using (TimeMeasuring.Run(false, this.ToString()))
            {
                LOGGER.Trace($"{this} を実行します。");
                var sw = Stopwatch.StartNew();
                try
                {
                    if (this.Parameter != null)
                    {
                        await this.Execute(this.Parameter);
                    }
                    else
                    {
                        await this.Execute();
                    }
                }
                catch (JobCancelException)
                {
                    LOGGER.Trace($"{this} がキャンセルされました。");
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (AppException ex)
                {
                    LOGGER.Error(ex, $"{this} で例外が発生しました。");
                }
                catch (Exception ex)
                {
                    LOGGER.Error(ex, $"{this} で補足されない例外が発生しました。");
                }
                finally
                {
                    sw.Stop();
                    LOGGER.Trace($"{this} が終了しました。{sw.ElapsedMilliseconds} ms");
                }
            }
        }

        protected virtual Task Execute(TParameter parameter)
        {
            throw new NotImplementedException();
        }

        protected virtual Task Execute()
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
        where TParameter : class, IJobParameter
    {

    }

    public abstract class AbstractOneWayJob
        : AbstractTwoWayJob<EmptyParameter, EmptyResult>
    {

    }
}
