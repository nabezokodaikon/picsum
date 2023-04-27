using PicSum.Core.Task.Base;
using System;
using System.Threading;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスク情報
    /// </summary>
    public class TaskInfo : IEquatable<TaskInfo>
    {
        #region クラスメンバ

        // 新しいタスクID
        private static int newTaskId = 0;

        // 新しいタスクIDを取得します。
        private static int GetNewTaskId()
        {
            return Interlocked.Increment(ref newTaskId);
        }

        #endregion

        #region イベント・デリゲート

        /// <summary>
        /// タスク状態変更イベント
        /// </summary>
        internal event EventHandler<TaskStateChangedEventArgs> TaskStateChanged;

        #endregion

        #region インスタンス変数

        private long endDateTimeTicks = 0;
        private readonly IEntity parameter;
        private long isExecuting = 0;
        private long isCancel = 0;
        private long isEnd = 0;
        private Exception exception = null;

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// 呼出元
        /// </summary>
        public object Sender { get; private set; }

        /// <summary>
        /// タスクID
        /// </summary>
        public int TaskId { get; private set; }

        /// <summary>
        /// タスク開始日時
        /// </summary>
        public DateTime StartDateTime { get; private set; }

        /// <summary>
        /// タスク終了日時
        /// </summary>
        public DateTime EndDateTime
        {
            get
            {
                var ticks = Interlocked.Read(ref this.endDateTimeTicks);
                return new DateTime(ticks);
            }
            set
            {
                var ticks = value.Ticks;
                Interlocked.Exchange(ref this.endDateTimeTicks, ticks);
            }
        }

        /// <summary>
        /// プロセスの型
        /// </summary>
        public Type ProcessType { get; private set; }

        /// <summary>
        /// 実行中フラグ
        /// </summary>
        public bool IsExecuting
        {
            get
            {
                return Interlocked.Read(ref this.isExecuting) == 1;
            }
            private set
            {
                Interlocked.Exchange(ref this.isExecuting, Convert.ToInt64(value));
            }
        }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        public bool IsCancel
        {
            get
            {
                return Interlocked.Read(ref this.isCancel) == 1;
            }
            private set
            {
                Interlocked.Exchange(ref this.isCancel, Convert.ToInt64(value));
            }
        }

        /// <summary>
        /// 終了フラグ
        /// </summary>
        public bool IsEnd
        {
            get
            {
                return Interlocked.Read(ref this.isEnd) == 1;
            }
            private set
            {
                Interlocked.Exchange(ref this.isEnd, Convert.ToInt64(value));
            }
        }

        /// <summary>
        /// 例外フラグ
        /// </summary>
        public bool IsError
        {
            get
            {
                var ex = Interlocked.CompareExchange<Exception>(ref this.exception, null, null);
                return ex != null;
            }
        }

        /// <summary>
        /// パラメータ
        /// </summary>
        public IEntity Parameter
        {
            get
            {
                if (parameter == null) throw new NullReferenceException();
                return parameter;
            }
        }

        /// <summary>
        /// 例外
        /// </summary>
        public Exception Exception
        {
            get
            {
                return Interlocked.CompareExchange<Exception>(ref this.exception, null, null);
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sender">呼出元</param>
        /// <param name="processType">プロセスの型</param>
        internal TaskInfo(object sender, Type processType)
        {
            this.Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.ProcessType = processType ?? throw new ArgumentNullException(nameof(processType));
            this.TaskId = TaskInfo.GetNewTaskId();
            this.StartDateTime = DateTime.Now;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sender">呼出元</param>
        /// <param name="processType">プロセスの型</param>
        /// <param name="param">パラメータ</param>
        internal TaskInfo(object sender, Type processType, IEntity param)
        {
            this.Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.ProcessType = processType ?? throw new ArgumentNullException(nameof(processType));
            this.parameter = param ?? throw new ArgumentNullException(nameof(param));
            this.TaskId = TaskInfo.GetNewTaskId();
            this.StartDateTime = DateTime.Now;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// キャンセルを試みます。
        /// </summary>
        public void BeginCancel()
        {
            if (this.IsEnd)
            {
                return;
            }

            if (this.IsCancel)
            {
                return;
            }

            this.CancelExecute();
        }

        /// <summary>
        /// タスクが実行されたことを通知します。
        /// </summary>
        internal void StartExecute()
        {
            if (this.IsEnd)
            {
                throw new Exception(string.Format("タスク[{0}]は終了しています。", this.TaskId));
            }

            if (this.IsExecuting)
            {
                throw new Exception(string.Format("タスク[{0}]は既に実行中です。", this.TaskId));
            }

            if (this.IsCancel)
            {
                throw new Exception(string.Format("タスク[{0}]はキャンセルされています。", this.TaskId));
            }

            this.IsExecuting = true;

            this.OnTaskStateChanged(new TaskStateChangedEventArgs(this));
        }

        /// <summary>
        /// タスクがキャンセルされたことを通知します。
        /// </summary>
        internal void CancelExecute()
        {
            if (this.IsEnd)
            {
                throw new Exception(string.Format("タスク[{0}]は終了しています。", this.TaskId));
            }

            if (this.IsCancel)
            {
                throw new Exception(string.Format("タスク[{0}]はキャンセルされています。", this.TaskId));
            }

            this.IsCancel = true;

            this.OnTaskStateChanged(new TaskStateChangedEventArgs(this));
        }

        /// <summary>
        /// タスクが完了されたことを通知します。
        /// </summary>
        internal void EndExecute()
        {
            if (this.IsEnd)
            {
                throw new Exception(string.Format("タスク[{0}]は終了しています。", this.TaskId));
            }

            this.IsEnd = true;
            this.EndDateTime = DateTime.Now;

            this.OnTaskStateChanged(new TaskStateChangedEventArgs(this));
        }

        /// <summary>
        /// 例外をセットします。
        /// </summary>
        /// <param name="ex"></param>
        internal void SetException(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));

            if (Interlocked.CompareExchange<Exception>(ref this.exception, ex, null) != null)
            {
                throw new Exception("既に例外がセットされています。");
            }

            this.OnTaskStateChanged(new TaskStateChangedEventArgs(this));
        }

        #region IEquatable<TaskInfo> メンバ

        public override int GetHashCode()
        {
            return this.TaskId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return this.Equals((TaskInfo)obj);
        }

        public bool Equals(TaskInfo other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.TaskId == other.TaskId);
        }

        #endregion

        #endregion

        #region プライベートメソッド

        // タスク状態変更イベントを発生させます。
        private void OnTaskStateChanged(TaskStateChangedEventArgs e)
        {
            if (this.TaskStateChanged != null)
            {
                this.TaskStateChanged(this, new TaskStateChangedEventArgs(this));
            }
        }

        #endregion
    }
}
