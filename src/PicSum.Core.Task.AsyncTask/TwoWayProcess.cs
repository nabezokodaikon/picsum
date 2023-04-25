﻿using NLog;
using PicSum.Core.Task.Base;
using SWF.Common;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// TwoWayプロセスクラス
    /// </summary>
    /// <typeparam name="TFacade">ファサードの型</typeparam>
    /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
    public sealed class TwoWayProcess<TFacade, TCallbackEventArgs> : ProcessBase
        where TFacade : TwoWayFacadeBase<TCallbackEventArgs>, new()
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
        protected override void ExecuteThread(TaskInfo task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            var facade = new TFacade();
            var facadeName = facade.GetType().Name;
            Thread.CurrentThread.Name = facadeName;
#if DEBUG
            Logger.Debug("タスクID: {0} Facade: {1} を開始します。", task.TaskId, facadeName);
#endif

            facade.Callback += new AsyncTaskCallbackEventHandler<TCallbackEventArgs>
                ((object sender, TCallbackEventArgs e) => this.SendMessageThread(this.OnCallback, new object[] { sender, e }));
            facade.SetTask(task);

            try
            {
                facade.Execute();
            }
            catch (TaskCancelException)
            {
#if DEBUG
                Logger.Debug("タスクID: {0} Facade: {1} がキャンセルされました。", task.TaskId, facadeName);
#endif
                this.SendMessageThread(this.OnCancelEnd, task);
                return;
            }
            catch (Exception ex)
            {
#if DEBUG
                Logger.Error("タスクID: {0} Facade: {1} で例外が発生しました。\n{2}", task.TaskId, facadeName, ExceptionUtil.CreateDetailsMessage(ex));
#endif
                this.SendMessageThread(this.OnErrorEnd, new object[] { task, ex });
                return;
            }
            finally
            {
#if DEBUG
                Logger.Debug("タスクID: {0} Facade: {1} が終了しました。", task.TaskId, facadeName);
#endif            
            }

            this.SendMessageThread(this.OnSuccessEnd, task);
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
    /// <typeparam name="TFacade">ファサードの型</typeparam>
    /// <typeparam name="TParameter">パラメータの型</typeparam>
    /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
    public sealed class TwoWayProcess<TFacade, TParameter, TCallbackEventArgs> : ProcessBase
        where TFacade : TwoWayFacadeBase<TParameter, TCallbackEventArgs>, new()
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
        protected override void ExecuteThread(TaskInfo task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            var facade = new TFacade();
            var facadeName = facade.GetType().Name;
            Thread.CurrentThread.Name = facadeName;
#if DEBUG
            Logger.Debug("タスクID: {0} Facade: {1} を開始します。", task.TaskId, facadeName);
#endif

            facade.Callback += new AsyncTaskCallbackEventHandler<TCallbackEventArgs>
                ((object sender, TCallbackEventArgs e) => this.SendMessageThread(this.OnCallback, new object[] { sender, e }));
            facade.SetTask(task);

            try
            {
                facade.Execute((TParameter)task.Parameter);
            }
            catch (TaskCancelException)
            {
#if DEBUG
                Logger.Debug("タスクID: {0} Facade: {1} がキャンセルされました。", task.TaskId, facadeName);
#endif
                this.SendMessageThread(this.OnCancelEnd, task);
                return;
            }
            catch (Exception ex)
            {
#if DEBUG
                Logger.Error("タスクID: {0} Facade: {1} で例外が発生しました。\n{2}", task.TaskId, facadeName, ExceptionUtil.CreateDetailsMessage(ex));
#endif
                this.SendMessageThread(this.OnErrorEnd, new object[] { task, ex });
                return;
            }
            finally
            {
#if DEBUG
                Logger.Debug("タスクID: {0} Facade: {1} が終了しました。", task.TaskId, facadeName);
#endif            
            }

            this.SendMessageThread(this.OnSuccessEnd, task);
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
