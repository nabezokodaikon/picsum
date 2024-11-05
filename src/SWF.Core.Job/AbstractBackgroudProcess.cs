using NLog;
using System.Collections.Concurrent;

namespace SWF.Core.Job
{
    public abstract class AbstractBackgroudProcess<TJob, TJobParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected readonly string ThreadName;
        protected readonly SynchronizationContext Context;
        protected readonly ConcurrentQueue<TJob> JobQueue = new();

        protected Action<TJobResult>? CallbackAction;
        protected Action? CancelAction;
        protected Action<JobException>? CatchAction;
        protected Action? CompleteAction;

        protected AbstractBackgroudProcess(SynchronizationContext? context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            this.Context = context;
            this.ThreadName = $"{typeof(TJob).Name} {ThreadID.GetNew()}";
        }

        public abstract void StartJob(ISender sender, TJobParameter parameter);
        public abstract void StartJob(ISender sender);

        public AbstractBackgroudProcess<TJob, TJobParameter, TJobResult> Reset()
        {
            this.CallbackAction = null;
            this.CancelAction = null;
            this.CatchAction = null;
            this.CompleteAction = null;

            return this;
        }

        public AbstractBackgroudProcess<TJob, TJobParameter, TJobResult> Callback(Action<TJobResult> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.CallbackAction != null)
            {
                throw new InvalidOperationException("コールバックアクションが初期化されていません。");
            }

            this.CallbackAction = action;
            return this;
        }

        public AbstractBackgroudProcess<TJob, TJobParameter, TJobResult> Cancel(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.CancelAction != null)
            {
                throw new InvalidOperationException("キャンセルアクションが初期化されていません。");
            }

            this.CancelAction = action;
            return this;
        }

        public AbstractBackgroudProcess<TJob, TJobParameter, TJobResult> Catch(Action<JobException> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.CatchAction != null)
            {
                throw new InvalidOperationException("例外アクションが初期化されていません。");
            }

            this.CatchAction = action;
            return this;
        }

        public AbstractBackgroudProcess<TJob, TJobParameter, TJobResult> Complete(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.CompleteAction != null)
            {
                throw new InvalidOperationException("完了アクションが初期化されていません。");
            }

            this.CompleteAction = action;
            return this;
        }

        public AbstractBackgroudProcess<TJob, TJobParameter, TJobResult> BeginCancel()
        {
            foreach (var job in this.JobQueue.ToArray())
            {
                job.BeginCancel();
            }

            return this;
        }

        public AbstractBackgroudProcess<TJob, TJobParameter, TJobResult> WaitJobComplete()
        {
            Logger.Debug("ジョブキューの完了を待ちます。");

            foreach (var job in this.JobQueue.ToArray())
            {
                while (!job.IsCompleted)
                {
                    Thread.Sleep(1);
                }
            }

            Logger.Debug("ジョブキューが完了しました。");

            return this;
        }

        protected TJob CreateJob()
        {
            var job = new TJob();

            if (this.CallbackAction != null)
            {
                var action = this.CallbackAction;
                job.CallbackAction = result =>
                {
                    if (job.CanUIThreadAccess())
                    {
                        this.Context.Post(state =>
                        {
#pragma warning disable CS8600
                            var objects = (object[])state;
#pragma warning restore CS8600
#pragma warning disable CS8602
                            var innerJob = (TJob)objects[0];
#pragma warning restore CS8602
                            var innerAction = (Action<TJobResult>)objects[1];
                            if (innerJob.CanUIThreadAccess())
                            {
                                innerAction.Invoke(result);
                            }
                        }, new object[] { job, action });
                    }
                };
            }

            if (this.CancelAction != null)
            {
                var action = this.CancelAction;
                job.CancelAction = () =>
                {
                    if (job.CanUIThreadAccess())
                    {
                        this.Context.Post(state =>
                        {
#pragma warning disable CS8600
                            var objects = (object[])state;
#pragma warning restore CS8600
#pragma warning disable CS8602
                            var innerJob = (TJob)objects[0];
#pragma warning restore CS8602
                            var innerAction = (Action)objects[1];
                            if (innerJob.CanUIThreadAccess())
                            {
                                innerAction.Invoke();
                            }
                        }, new object[] { job, action });
                    }
                };
            }

            if (this.CatchAction != null)
            {
                var action = this.CatchAction;
                job.CatchAction = exception =>
                {
                    if (job.CanUIThreadAccess())
                    {
                        this.Context.Post(state =>
                        {
#pragma warning disable CS8600
                            var objects = (object[])state;
#pragma warning restore CS8600
#pragma warning disable CS8602
                            var innerJob = (TJob)objects[0];
#pragma warning restore CS8602
                            var innerAction = (Action<JobException>)objects[1];
                            if (innerJob.CanUIThreadAccess())
                            {
                                innerAction.Invoke(exception);
                            }
                        }, new object[] { job, action });
                    }
                };
            }

            if (this.CompleteAction != null)
            {
                var action = this.CompleteAction;
                job.CompleteAction = () =>
                {
                    if (job.CanUIThreadAccess())
                    {
                        this.Context.Post(state =>
                        {
#pragma warning disable CS8600
                            var objects = (object[])state;
#pragma warning restore CS8600
#pragma warning disable CS8602
                            var innerJob = (TJob)objects[0];
#pragma warning restore CS8602
                            var innerAction = (Action)objects[1];
                            if (innerJob.CanUIThreadAccess())
                            {
                                innerAction.Invoke();
                            }
                        }, new object[] { job, action });
                    }
                };
            }

            return job;
        }
    }
}
