using NLog;
using PicSum.Core.Task.Base;
using SWF.Common;
using System;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Threading;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// TwoWayプロセスクラス
    /// </summary>
    /// <typeparam name="TTask">タスクの型</typeparam>
    /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
    [SupportedOSPlatform("windows")]
    public sealed class TwoWayProcess<TTask, TCallbackEventArgs> : ProcessBase
        where TTask : TwoWayTaskBase<TCallbackEventArgs>, new()
        where TCallbackEventArgs : IEntity
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コールバックイベント
        /// </summary>
        public event AsyncTaskCallbackEventHandler<TCallbackEventArgs> Callback;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container">コンテナ</param>
        internal TwoWayProcess(IContainer container)
            : base(container) { }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="sender">呼出元</param>
        public void Execute(object sender)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));

            this.CreateNewTask(sender);
        }

        // 実行スレッド。
        protected override void ExecuteThread(TaskInfo taskInfo)
        {
            if (taskInfo == null) throw new ArgumentNullException(nameof(taskInfo));

            var task = new TTask();
            var taskName = task.GetType().Name;
            Thread.CurrentThread.Name = taskName;
#if DEBUG
            Logger.Debug("TaskID: {0}, Task: {1} を開始します。", taskInfo.TaskId, taskName);
#endif

            task.Callback += new AsyncTaskCallbackEventHandler<TCallbackEventArgs>
                ((object sender, TCallbackEventArgs e) => this.SendToUIThread(this.OnCallback, new object[] { sender, e }));
            task.SetTask(taskInfo);

            try
            {
                task.Execute();
            }
            catch (TaskCancelException)
            {
#if DEBUG
                Logger.Debug("TaskID: {0}, Task: {1} がキャンセルされました。", taskInfo.TaskId, taskName);
#endif
                this.SendToUIThread(this.OnCancelEnd, taskInfo);
                return;
            }
            catch (Exception ex)
            {
#if DEBUG
                Logger.Error("TaskID: {0}, Task: {1} で例外が発生しました。\n{2}", taskInfo.TaskId, taskName, ExceptionUtil.CreateDetailsMessage(ex));
#endif
                this.SendToUIThread(this.OnErrorEnd, new object[] { taskInfo, ex });
                return;
            }
            finally
            {
#if DEBUG
                Logger.Debug("TaskID: {0}, Task: {1} が終了しました。", taskInfo.TaskId, taskName);
#endif            
            }

            this.SendToUIThread(this.OnSuccessEnd, taskInfo);
        }

        // コールバックイベントを発生させます。
        private void OnCallback(object obj)
        {
            var args = (object[])obj;
            var task = (TaskInfo)args[0];
            var e = (TCallbackEventArgs)args[1];

            if (this.Callback != null)
            {
                this.Callback(this, e);
            }
        }
    }

    /// <summary>
    /// TwoWayプロセスクラス
    /// </summary>
    /// <typeparam name="TTask">タスクの型</typeparam>
    /// <typeparam name="TParameter">パラメータの型</typeparam>
    /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
    [SupportedOSPlatform("windows")]
    public sealed class TwoWayProcess<TTask, TParameter, TCallbackEventArgs> : ProcessBase
        where TTask : TwoWayTaskBase<TParameter, TCallbackEventArgs>, new()
        where TParameter : IEntity
        where TCallbackEventArgs : IEntity
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コールバックイベント
        /// </summary>
        public event AsyncTaskCallbackEventHandler<TCallbackEventArgs> Callback;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container">コンテナ</param>
        internal TwoWayProcess(IContainer container) : base(container) { }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="sender">呼出元</param>
        /// <param name="param">パラメータ</param>
        public void Execute(object sender, TParameter param)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (param == null) throw new ArgumentNullException(nameof(param));

            this.CreateNewTask(sender, param);
        }

        // 実行スレッド。
        protected override void ExecuteThread(TaskInfo taskInfo)
        {
            if (taskInfo == null) throw new ArgumentNullException(nameof(taskInfo));

            var task = new TTask();
            var taskName = task.GetType().Name;
            Thread.CurrentThread.Name = taskName;
#if DEBUG
            Logger.Debug("TaskID: {0}, Task: {1} を開始します。", taskInfo.TaskId, taskName);
#endif

            task.Callback += new AsyncTaskCallbackEventHandler<TCallbackEventArgs>
                ((object sender, TCallbackEventArgs e) => this.SendToUIThread(this.OnCallback, new object[] { sender, e }));
            task.SetTask(taskInfo);

            try
            {
                task.Execute((TParameter)taskInfo.Parameter);
            }
            catch (TaskCancelException)
            {
#if DEBUG
                Logger.Debug("TaskID: {0}, Task: {1} がキャンセルされました。", taskInfo.TaskId, taskName);
#endif
                this.SendToUIThread(this.OnCancelEnd, taskInfo);
                return;
            }
            catch (Exception ex)
            {
#if DEBUG
                Logger.Error("TaskID: {0}, Task: {1} で例外が発生しました。\n{2}", taskInfo.TaskId, taskName, ExceptionUtil.CreateDetailsMessage(ex));
#endif
                this.SendToUIThread(this.OnErrorEnd, new object[] { taskInfo, ex });
                return;
            }
            finally
            {
#if DEBUG
                Logger.Debug("TaskID: {0}, Task: {1} が終了しました。", taskInfo.TaskId, taskName);
#endif            
            }

            this.SendToUIThread(this.OnSuccessEnd, taskInfo);
        }

        // コールバックイベントを発生させます。
        private void OnCallback(object obj)
        {
            var args = (object[])obj;
            var task = (TaskInfo)args[0];
            var e = (TCallbackEventArgs)args[1];

            if (this.Callback != null)
            {
                this.Callback(this, e);
            }
        }
    }
}
