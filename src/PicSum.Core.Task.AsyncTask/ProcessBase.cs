using PicSum.Core.Task.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

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
    public abstract class ProcessBase
        : Component
    {
        private bool disposed = false;

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
        private readonly List<TaskInfo> taskList = new List<TaskInfo>();

        // コンテキスト
        private readonly SynchronizationContext context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container">コンテナ</param>
        protected ProcessBase(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.Add(this);

            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }

            this.context = SynchronizationContext.Current;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Cancel();
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        public new void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// タスクを全てキャンセルします。
        /// </summary>
        public void Cancel()
        {
            foreach (var task in this.taskList)
            {
                if (!task.IsCancel && !task.IsEnd)
                {
                    task.CancelExecute();
                }
            }

            this.taskList.Clear();
        }

        /// <summary>
        /// 新しいタスクを作成し、処理を実行します。
        /// </summary>
        /// <param name="sender">呼出元</param>
        /// <param name="param">パラメータ</param>
        protected void CreateNewTask(object sender, IEntity param)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (param == null) throw new ArgumentNullException(nameof(param));

            var task = TaskManager.CreateNewTask(sender, this.GetType(), param);

            this.taskList.Add(task);

            if (!this.HasExecutingTask())
            {
                if (task.Equals(this.GetNextTask()))
                {
                    this.StartExecuteThread(task);
                }
            }
        }

        /// <summary>
        /// 新しいタスクを作成し、処理を実行します。
        /// </summary>
        /// <param name="sender">呼出元</param>
        protected void CreateNewTask(object sender)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));

            var task = TaskManager.CreateNewTask(sender, this.GetType());
            this.taskList.Add(task);

            if (!this.HasExecutingTask())
            {
                if (task.Equals(GetNextTask()))
                {
                    this.StartExecuteThread(task);
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
            if (d == null) throw new ArgumentNullException(nameof(d));
            if (state == null) throw new ArgumentNullException(nameof(state));

            this.context.Send(d, state);
        }

        /// <summary>
        /// 実行スレッドを開始します。
        /// </summary>
        /// <param name="task">タスク</param>
        protected void StartExecuteThread(TaskInfo task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            task.StartExecute();
            var d = new Action<TaskInfo>(this.ExecuteThread);
            d.BeginInvoke(task, null, null);
        }

        /// <summary>
        /// 成功終了イベントを発生させます。
        /// </summary>
        /// <param name="obj">タスク</param>
        protected void OnSuccessEnd(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var task = (TaskInfo)obj;

            this.taskList.Remove(task);

            task.EndExecute();

            if (this.SuccessEnd != null)
            {
                this.SuccessEnd(this, new EventArgs());
            }

            if (!this.HasExecutingTask())
            {
                var nextTask = GetNextTask();

                if (nextTask != null)
                {
                    this.StartExecuteThread(nextTask);
                }
            }
        }

        /// <summary>
        /// キャンセル終了イベントを発生させます。
        /// </summary>
        /// <param name="obj">タスク</param>
        protected void OnCancelEnd(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var task = (TaskInfo)obj;

            this.taskList.Remove(task);

            task.EndExecute();

            if (this.CancelEnd != null)
            {
                this.CancelEnd(this, new EventArgs());
            }

            if (!this.HasExecutingTask())
            {
                var nextTask = GetNextTask();

                if (nextTask != null)
                {
                    this.StartExecuteThread(nextTask);
                }
            }
        }

        /// <summary>
        /// 例外終了イベントを発生させます。
        /// </summary>
        /// <param name="obj"></param>
        protected void OnErrorEnd(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var args = (object[])obj;
            var task = (TaskInfo)args[0];
            task.SetException((Exception)args[1]);

            this.taskList.Remove(task);

            task.EndExecute();

            if (this.ErrorEnd != null)
            {
                this.ErrorEnd(this, new EventArgs());
            }

            if (!this.HasExecutingTask())
            {
                var nextTask = GetNextTask();

                if (nextTask != null)
                {
                    StartExecuteThread(nextTask);
                }
            }
        }

        // 実行中のタスクの存在を確認します。
        private bool HasExecutingTask()
        {
            foreach (var task in taskList)
            {
                if (task.IsExecuting)
                {
                    return true;
                }
            }

            return false;
        }

        // 次のタスクを取得します。
        private TaskInfo GetNextTask()
        {
            foreach (var task in taskList)
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
