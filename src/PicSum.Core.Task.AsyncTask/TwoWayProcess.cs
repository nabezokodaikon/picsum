using System;
using System.ComponentModel;
using System.Windows.Forms;
using PicSum.Core.Task.Base;

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
        public void Execute(object sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            CreateNewTask(sender);
        }

        // 実行スレッド。
        protected override void ExecuteThread(TaskInfo task)
        {
            TFacade facade = new TFacade();
            facade.Callback += new AsyncTaskCallbackEventHandler<TCallbackEventArgs>
                ((object sender, TCallbackEventArgs e) => SendMessageThread(OnCallback, new object[] { sender, e }));
            facade.SetTask(task);

            try
            {
                facade.Execute();
            }
            catch (TaskCancelException)
            {
                SendMessageThread(OnCancelEnd, task);
                return;
            }
            catch (Exception ex)
            {
                SendMessageThread(OnErrorEnd, new object[] { task, ex });
                return;
            }

            SendMessageThread(OnSuccessEnd, task);
        }

        // コールバックイベントを発生させます。
        private void OnCallback(object obj)
        {
            object[] args = (object[])obj;
            TaskInfo task = (TaskInfo)args[0];
            TCallbackEventArgs e = (TCallbackEventArgs)args[1];

            if (Callback != null)
            {
                if (task.Sender is Control ctl)
                {
                    if (ctl.IsHandleCreated)
                    {
                        Callback(this, e);
                    }
                }
                else
                {
                    Callback(this, e);
                }
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
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            CreateNewTask(sender, param);
        }

        // 実行スレッド。
        protected override void ExecuteThread(TaskInfo task)
        {
            TFacade facade = new TFacade();
            facade.Callback += new AsyncTaskCallbackEventHandler<TCallbackEventArgs>
                ((object sender, TCallbackEventArgs e) => SendMessageThread(OnCallback, new object[] { sender, e }));
            facade.SetTask(task);

            try
            {
                facade.Execute((TParameter)task.Parameter);
            }
            catch (TaskCancelException)
            {
                SendMessageThread(OnCancelEnd, task);
                return;
            }
            catch (Exception ex)
            {
                SendMessageThread(OnErrorEnd, new object[] { task, ex });
                return;
            }

            SendMessageThread(OnSuccessEnd, task);
        }

        // コールバックイベントを発生させます。
        private void OnCallback(object obj)
        {
            object[] args = (object[])obj;
            TaskInfo task = (TaskInfo)args[0];
            TCallbackEventArgs e = (TCallbackEventArgs)args[1];

            if (Callback != null)
            {
                if (task.Sender is Control ctl)
                {
                    if (ctl.IsHandleCreated)
                    {
                        Callback(this, e);
                    }
                }
                else
                {
                    Callback(this, e);
                }
            }
        }
    }
}
