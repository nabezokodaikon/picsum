using PicSum.Core.Task.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Versioning;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスク管理
    /// </summary>
    /// <remarks>UIスレッドより使用してください。</remarks>
    [SupportedOSPlatform("windows")]
    public static class TaskManager
    {
        #region プロセスインスタンス作成

        /// <summary>
        /// プロセスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="TTask">タスクの型</typeparam>
        /// <param name="container">コンテナ</param>
        /// <returns>プロセス</returns>
        public static OneWayProcess<TTask> CreateOneWayProcess<TTask>(IContainer container)
            where TTask : OneWayTaskBase, new()
        {
            return new OneWayProcess<TTask>(container);
        }

        /// <summary>
        /// プロセスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="TTask">タスクの型</typeparam>
        /// <typeparam name="TParameter">パラメータの型</typeparam>
        /// <param name="container">コンテナ</param>
        /// <returns>プロセス</returns>
        public static OneWayProcess<TTask, TParameter> CreateOneWayProcess<TTask, TParameter>(IContainer container)
            where TTask : OneWayTaskBase<TParameter>, new()
            where TParameter : IEntity
        {
            return new OneWayProcess<TTask, TParameter>(container);
        }

        /// <summary>
        /// プロセスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="TTask">タスクの型</typeparam>
        /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
        /// <param name="container">コンテナ</param>
        /// <returns>プロセス</returns>
        public static TwoWayProcess<TTask, TCallbackEventArgs> CreateTwoWayProcess<TTask, TCallbackEventArgs>(IContainer container)
            where TTask : TwoWayTaskBase<TCallbackEventArgs>, new()
            where TCallbackEventArgs : IEntity
        {
            return new TwoWayProcess<TTask, TCallbackEventArgs>(container);
        }

        /// <summary>
        /// プロセスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="TTask">タスクの型</typeparam>
        /// <typeparam name="TParameter">パラメータの型</typeparam>
        /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
        /// <param name="container">コンテナ</param>
        /// <returns>プロセス</returns>
        public static TwoWayProcess<TTask, TParameter, TCallbackEventArgs> CreateTwoWayProcess<TTask, TParameter, TCallbackEventArgs>(IContainer container)
            where TTask : TwoWayTaskBase<TParameter, TCallbackEventArgs>, new()
            where TParameter : IEntity
            where TCallbackEventArgs : IEntity
        {
            return new TwoWayProcess<TTask, TParameter, TCallbackEventArgs>(container);
        }

        #endregion

        #region タスク操作

        /// <summary>
        /// タスク変更イベント
        /// </summary>
        public static event EventHandler<TaskStateChangedEventArgs> TaskStateChanged;

        /// <summary>
        /// 現在のタスク一覧を取得します。
        /// </summary>
        public readonly static List<TaskInfo> TaskList = new List<TaskInfo>();

        /// <summary>
        /// 新しいタスクを作成します。
        /// </summary>
        /// <typeparam name="TParameter">パラメータの型</typeparam>
        /// <param name="sender">呼出元</param>
        /// <param name="processType">プロセスの型</param>
        /// <param name="param">パラメータ</param>
        /// <returns>タスク</returns>
        internal static TaskInfo CreateNewTask<TParameter>(
            object sender, Type processType, TParameter param) where TParameter : IEntity
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (processType == null) throw new ArgumentNullException(nameof(processType));
            if (param == null) throw new ArgumentNullException(nameof(param));

            var task = new TaskInfo(sender, processType, param);

            TaskManager.TaskList.Add(task);
            TaskManager.OnTaskStateChanged(new TaskStateChangedEventArgs(task));
            TaskManager.SetupTaskDelegate(task);

            return task;
        }

        /// <summary>
        /// 新しいタスクを作成します。
        /// </summary>
        /// <param name="sender">呼出元</param>
        /// <param name="processType">プロセスの型</param>
        /// <returns>タスク</returns>
        internal static TaskInfo CreateNewTask(object sender, Type processType)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (processType == null) throw new ArgumentNullException(nameof(processType));

            var task = new TaskInfo(sender, processType);

            TaskManager.TaskList.Add(task);
            TaskManager.OnTaskStateChanged(new TaskStateChangedEventArgs(task));
            TaskManager.SetupTaskDelegate(task);

            return task;
        }

        // タスクのデリゲートをセットします。
        private static void SetupTaskDelegate(TaskInfo task)
        {
            task.TaskStateChanged += new EventHandler<TaskStateChangedEventArgs>(TaskManager.task_TaskStateChanged);
        }

        // タスク タスク状態変更イベント
        private static void task_TaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            if (e.Task.IsEnd || e.Task.IsCancel)
            {
                // 終了または、キャンセルされた場合
                TaskManager.TaskList.Remove(e.Task);
            }

            TaskManager.OnTaskStateChanged(new TaskStateChangedEventArgs(e.Task));
        }

        // タスク変更イベントを発生させます。
        private static void OnTaskStateChanged(TaskStateChangedEventArgs e)
        {
            if (TaskManager.TaskStateChanged != null)
            {
                TaskManager.TaskStateChanged(null, e);
            }
        }

        #endregion
    }
}
