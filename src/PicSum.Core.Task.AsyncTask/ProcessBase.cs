using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using PicSum.Core.Base.Log;
using PicSum.Core.Task.Base;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// 非同期タスクコールバックイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AsyncTaskCallbackEventHandler<TAsyncTaskCallbackEventArgs>
        (object sender, TAsyncTaskCallbackEventArgs e) where TAsyncTaskCallbackEventArgs : IEntity;

    /// <summary>
    /// プロセス基底クラス
    /// </summary>
    public abstract class ProcessBase : Component
    {
        /// <summary>
        /// 成功終了イベント
        /// </summary>
        public event EventHandler SuccessEnd;

        /// <summary>
        /// キャンセル終了イベント
        /// </summary>
        public event EventHandler CancelEnd;

        /// <summary>
        /// 例外終了イベント
        /// </summary>
        public event EventHandler ErrorEnd;

        // タスクリスト
        private readonly List<TaskInfo> _taskList = new List<TaskInfo>();

        // コンテキスト
        private readonly SynchronizationContext _context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container">コンテナ</param>
        protected ProcessBase(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            container.Add(this);

            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }

            _context = SynchronizationContext.Current;
        }

        /// <summary>
        /// タスクを全てキャンセルします。
        /// </summary>
        public void Cancel()
        {
            foreach (TaskInfo task in _taskList)
            {
                if (!task.IsCancel && !task.IsEnd)
                {
                    task.CancelExecute();
                }
            }

            _taskList.Clear();
        }

        /// <summary>
        /// 新しいタスクを作成し、処理を実行します。
        /// </summary>
        /// <param name="sender">呼出元</param>
        /// <param name="param">パラメータ</param>
        protected void CreateNewTask(object sender, IEntity param)
        {
            TaskInfo task = TaskManager.CreateNewTask(sender, this.GetType(), param);

            _taskList.Add(task);

            if (!hasExecutingTask())
            {
                if (task.Equals(getNextTask()))
                {
                    StartExecuteThread(task);
                }
            }
        }

        /// <summary>
        /// 新しいタスクを作成し、処理を実行します。
        /// </summary>
        /// <param name="sender">呼出元</param>
        protected void CreateNewTask(object sender)
        {
            TaskInfo task = TaskManager.CreateNewTask(sender, this.GetType());

            _taskList.Add(task);

            if (!hasExecutingTask())
            {
                if (task.Equals(getNextTask()))
                {
                    StartExecuteThread(task);
                }
            }
        }

        /// <summary>
        /// 実行スレッドを開始します。
        /// </summary>
        /// <param name="task">タスク</param>
        protected abstract void ExecuteThread(TaskInfo task);

        /// <summary>
        /// メッセージスレッドに処理を委譲します。
        /// </summary>
        /// <param name="d">コールバックデリゲート</param>
        /// <param name="state">状態</param>
        protected void SendMessageThread(SendOrPostCallback d, object state)
        {
            _context.Send(d, state);
        }

        /// <summary>
        /// 実行スレッドを開始します。
        /// </summary>
        /// <param name="task">タスク</param>
        protected void StartExecuteThread(TaskInfo task)
        {
            task.StartExecute();
            Action<TaskInfo> d = new Action<TaskInfo>(ExecuteThread);
            d.BeginInvoke(task, null, null);
        }

        /// <summary>
        /// 成功終了イベントを発生させます。
        /// </summary>
        /// <param name="obj">タスク</param>
        protected void OnSuccessEnd(object obj)
        {
            TaskInfo task = (TaskInfo)obj;

            LogWriter.WriteLog(task.GetLogText());

            _taskList.Remove(task);

            task.EndExecute();

            if (SuccessEnd != null)
            {
                if (task.Sender is Control)
                {
                    Control ctl = (Control)task.Sender;
                    if (ctl.IsHandleCreated)
                    {
                        SuccessEnd(this, new EventArgs());
                    }
                }
                else
                {
                    SuccessEnd(this, new EventArgs());
                }
            }

            if (!hasExecutingTask())
            {
                TaskInfo nextTask = getNextTask();

                if (nextTask != null)
                {
                    StartExecuteThread(nextTask);
                }
            }
        }

        /// <summary>
        /// キャンセル終了イベントを発生させます。
        /// </summary>
        /// <param name="obj">タスク</param>
        protected void OnCancelEnd(object obj)
        {
            TaskInfo task = (TaskInfo)obj;

            LogWriter.WriteLog(task.GetLogText());

            _taskList.Remove(task);

            task.EndExecute();

            if (CancelEnd != null)
            {
                if (task.Sender is Control)
                {
                    Control ctl = (Control)task.Sender;
                    if (ctl.IsHandleCreated)
                    {
                        CancelEnd(this, new EventArgs());
                    }
                }
                else
                {
                    CancelEnd(this, new EventArgs());
                }
            }

            if (!hasExecutingTask())
            {
                TaskInfo nextTask = getNextTask();

                if (nextTask != null)
                {
                    StartExecuteThread(nextTask);
                }
            }
        }

        /// <summary>
        /// 例外終了イベントを発生させます。
        /// </summary>
        /// <param name="obj"></param>
        protected void OnErrorEnd(object obj)
        {
            object[] args = (object[])obj;
            TaskInfo task = (TaskInfo)args[0];
            task.SetException((Exception)args[1]);

            LogWriter.WriteLog(task.GetLogText());

            _taskList.Remove(task);

            task.EndExecute();

            if (ErrorEnd != null)
            {
                if (task.Sender is Control)
                {
                    Control ctl = (Control)task.Sender;
                    if (ctl.IsHandleCreated)
                    {
                        ErrorEnd(this, new EventArgs());
                    }
                }
                else
                {
                    ErrorEnd(this, new EventArgs());
                }
            }

            if (!hasExecutingTask())
            {
                TaskInfo nextTask = getNextTask();

                if (nextTask != null)
                {
                    StartExecuteThread(nextTask);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            Cancel();

            base.Dispose(disposing);
        }

        // 実行中のタスクの存在を確認します。
        private bool hasExecutingTask()
        {
            foreach (TaskInfo task in _taskList)
            {
                if (task.IsExecuting)
                {
                    return true;
                }
            }

            return false;
        }

        // 次のタスクを取得します。
        private TaskInfo getNextTask()
        {
            foreach (TaskInfo task in _taskList)
            {
                if (!task.IsCancel)
                {
                    return task;
                }
            }

            return null;
        }
    }
}
