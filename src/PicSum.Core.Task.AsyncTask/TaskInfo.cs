using System;
using System.Threading;
using PicSum.Core.Base.Exception;
using PicSum.Core.Task.Base;
using System.Text;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスク情報
    /// </summary>
    public class TaskInfo : IEquatable<TaskInfo>
    {
        #region クラスメンバ

        // 新しいタスクID
        private static int _newTaskId = 0;

        // 新しいタスクIDを取得します。
        private static int GetNewTaskId()
        {
            return Interlocked.Increment(ref _newTaskId);
        }

        #endregion

        #region イベント・デリゲート

        /// <summary>
        /// タスク状態変更イベント
        /// </summary>
        internal event EventHandler<TaskStateChangedEventArgs> TaskStateChanged;

        #endregion

        #region インスタンス変数

        private readonly int _taskId;
        private readonly DateTime _startDateTime;
        private long _endDateTimeTicks = 0;
        private readonly Type _processType = null;
        private readonly object _sender = null;
        private readonly IEntity _parameter;
        private long _isExecuting = 0;
        private long _isCancel = 0;
        private long _isEnd = 0;
        private Exception _exception = null;

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// 呼出元
        /// </summary>
        public object Sender
        {
            get
            {
                return _sender;
            }
        }

        /// <summary>
        /// タスクID
        /// </summary>
        public int TaskId
        {
            get
            {
                return _taskId;
            }
        }

        /// <summary>
        /// タスク開始日時
        /// </summary>
        public DateTime StartDateTime
        {
            get
            {
                return _startDateTime;
            }
        }

        /// <summary>
        /// タスク終了日時
        /// </summary>
        public DateTime EndDateTime
        {
            get
            {
                var ticks = Interlocked.Read(ref _endDateTimeTicks);
                return new DateTime(ticks);
            }
            set
            {
                var ticks = value.Ticks;
                Interlocked.Exchange(ref _endDateTimeTicks, ticks);
            }
        }

        /// <summary>
        /// プロセスの型
        /// </summary>
        public Type ProcessType
        {
            get
            {
                return _processType;
            }
        }

        /// <summary>
        /// 実行中フラグ
        /// </summary>
        public bool IsExecuting
        {
            get
            {
                return Interlocked.Read(ref _isExecuting) == 1;
            }
            private set
            {
                Interlocked.Exchange(ref _isExecuting, Convert.ToInt64(value));
            }
        }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        public bool IsCancel
        {
            get
            {
                return Interlocked.Read(ref _isCancel) == 1;
            }
            private set
            {
                Interlocked.Exchange(ref _isCancel, Convert.ToInt64(value));
            }
        }

        /// <summary>
        /// 終了フラグ
        /// </summary>
        public bool IsEnd
        {
            get
            {
                return Interlocked.Read(ref _isEnd) == 1;
            }
            private set
            {
                Interlocked.Exchange(ref _isEnd, Convert.ToInt64(value));
            }
        }

        /// <summary>
        /// 例外フラグ
        /// </summary>
        public bool IsError
        {
            get
            {
                var ex = Interlocked.CompareExchange<Exception>(ref _exception, null, null);
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
                if (_parameter == null)
                {
                    throw new NullReferenceException();
                }

                return _parameter;
            }
        }

        /// <summary>
        /// 例外
        /// </summary>
        public Exception Exception
        {
            get
            {
                return Interlocked.CompareExchange<Exception>(ref _exception, null, null);
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
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            if (processType == null)
            {
                throw new ArgumentNullException("processType");
            }

            _sender = sender;
            _taskId = GetNewTaskId();
            _startDateTime = DateTime.Now;
            _processType = processType;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sender">呼出元</param>
        /// <param name="processType">プロセスの型</param>
        /// <param name="param">パラメータ</param>
        internal TaskInfo(object sender, Type processType, IEntity param)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            if (processType == null)
            {
                throw new ArgumentNullException("processType");
            }

            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            _sender = sender;
            _taskId = GetNewTaskId();
            _startDateTime = DateTime.Now;
            _processType = processType;
            _parameter = param;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// キャンセルを試みます。
        /// </summary>
        public void BeginCancel()
        {
            if (IsEnd)
            {
                return;
            }

            if (IsCancel)
            {
                return;
            }

            CancelExecute();
        }

        /// <summary>
        /// タスクが実行されたことを通知します。
        /// </summary>
        internal void StartExecute()
        {
            if (IsEnd)
            {
                throw new Exception(string.Format("タスク[{0}]は終了しています。", _taskId));
            }

            if (IsExecuting)
            {
                throw new Exception(string.Format("タスク[{0}]は既に実行中です。", _taskId));
            }

            if (IsCancel)
            {
                throw new Exception(string.Format("タスク[{0}]はキャンセルされています。", _taskId));
            }

            IsExecuting = true;

            onTaskStateChanged(new TaskStateChangedEventArgs(this));
        }

        /// <summary>
        /// タスクがキャンセルされたことを通知します。
        /// </summary>
        internal void CancelExecute()
        {
            if (IsEnd)
            {
                throw new Exception(string.Format("タスク[{0}]は終了しています。", _taskId));
            }

            if (IsCancel)
            {
                throw new Exception(string.Format("タスク[{0}]はキャンセルされています。", _taskId));
            }

            IsCancel = true;

            onTaskStateChanged(new TaskStateChangedEventArgs(this));
        }

        /// <summary>
        /// タスクが完了されたことを通知します。
        /// </summary>
        internal void EndExecute()
        {
            if (IsEnd)
            {
                throw new Exception(string.Format("タスク[{0}]は終了しています。", _taskId));
            }

            IsEnd = true;
            EndDateTime = DateTime.Now;

            onTaskStateChanged(new TaskStateChangedEventArgs(this));
        }

        /// <summary>
        /// 例外をセットします。
        /// </summary>
        /// <param name="ex"></param>
        internal void SetException(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            if (Interlocked.CompareExchange<Exception>(ref _exception, ex, null) != null)
            {
                throw new Exception("既に例外がセットされています。");
            }

            onTaskStateChanged(new TaskStateChangedEventArgs(this));
        }

        /// <summary>
        /// ログ出力用の文字列を取得します。
        /// </summary>
        /// <returns></returns>
        public string GetLogText()
        {
            StringBuilder log = new StringBuilder();

            log.AppendFormat("タスクID：{0}\n", TaskId);

            log.AppendFormat("開始日時：{0:HH:mm:ss.fff}\n", StartDateTime);

            log.AppendFormat("終了日時：{0:HH:mm:ss.fff}\n", EndDateTime);

            log.AppendFormat("プロセス：{0}\n", ProcessType.ToString());

            if (_parameter != null)
            {
                log.AppendFormat("パラメータ：{0}\n", Parameter.ToString());
            }

            log.AppendFormat("呼出元：{0}\n", Sender.ToString());

            log.AppendFormat("実行中フラグ：{0}\n", IsExecuting);

            log.AppendFormat("終了フラグ：{0}\n", IsEnd);

            log.AppendFormat("キャンセルフラグ：{0}\n", IsCancel);

            if (IsError)
            {
                log.AppendFormat("Exception：{0}\n", Exception.GetType().ToString());

                if (!string.IsNullOrEmpty(Exception.Message))
                {
                    log.AppendFormat("Message：{0}\n", Exception.Message);
                }

                if (!string.IsNullOrEmpty(Exception.StackTrace))
                {
                    log.AppendFormat("StackTrace：{0}\n", Exception.StackTrace);
                }
            }

            return log.ToString();
        }

        /// <summary>
        /// ToStringメソッドをオーバーライドします。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string state;

            if (IsError)
            {
                state = "例外";
            }
            if (IsEnd)
            {
                state = "終了";
            }
            else if (IsCancel && IsExecuting)
            {
                state = "実行キャンセル";
            }
            else if (IsCancel)
            {
                state = "キャンセル";
            }
            else if (IsExecuting)
            {
                state = "実行中";
            }
            else
            {
                state = "待機";
            }

            return string.Format("[{0}]{1}", state, ProcessType.ToString());
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
        private void onTaskStateChanged(TaskStateChangedEventArgs e)
        {
            if (TaskStateChanged != null)
            {
                TaskStateChanged(this, new TaskStateChangedEventArgs(this));
            }
        }

        #endregion
    }
}
