using System;
using System.Collections.Generic;
using System.ComponentModel;
using PicSum.Core.Task.Base;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスク管理
    /// </summary>
    /// <remarks>メッセージスレッドより使用してください。</remarks>
    public static class TaskManager
    {
        #region プロセスインスタンス作成

        /// <summary>
        /// プロセスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="TFacade">ファサードの型</typeparam>
        /// <param name="container">コンテナ</param>
        /// <returns>プロセス</returns>
        public static OneWayProcess<TFacade> CreateOneWayProcess<TFacade>(IContainer container)
            where TFacade : OneWayFacadeBase, new()
        {
            return new OneWayProcess<TFacade>(container);
        }

        /// <summary>
        /// プロセスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="TFacade">ファサードの型</typeparam>
        /// <typeparam name="TParameter">パラメータの型</typeparam>
        /// <param name="container">コンテナ</param>
        /// <returns>プロセス</returns>
        public static OneWayProcess<TFacade, TParameter> CreateOneWayProcess<TFacade, TParameter>(IContainer container)
            where TFacade : OneWayFacadeBase<TParameter>, new()
            where TParameter : IEntity
        {
            return new OneWayProcess<TFacade, TParameter>(container);
        }

        /// <summary>
        /// プロセスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="TFacade">ファサードの型</typeparam>
        /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
        /// <param name="container">コンテナ</param>
        /// <returns>プロセス</returns>
        public static TwoWayProcess<TFacade, TCallbackEventArgs> CreateTwoWayProcess<TFacade, TCallbackEventArgs>(IContainer container)
            where TFacade : TwoWayFacadeBase<TCallbackEventArgs>, new()
            where TCallbackEventArgs : IEntity
        {
            return new TwoWayProcess<TFacade, TCallbackEventArgs>(container);
        }

        /// <summary>
        /// プロセスのインスタンスを作成します。
        /// </summary>
        /// <typeparam name="TFacade">ファサードの型</typeparam>
        /// <typeparam name="TParameter">パラメータの型</typeparam>
        /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
        /// <param name="container">コンテナ</param>
        /// <returns>プロセス</returns>
        public static TwoWayProcess<TFacade, TParameter, TCallbackEventArgs> CreateTwoWayProcess<TFacade, TParameter, TCallbackEventArgs>(IContainer container)
            where TFacade : TwoWayFacadeBase<TParameter, TCallbackEventArgs>, new()
            where TParameter : IEntity
            where TCallbackEventArgs : IEntity
        {
            return new TwoWayProcess<TFacade, TParameter, TCallbackEventArgs>(container);
        }

        #endregion

        #region タスク操作

        /// <summary>
        /// タスク変更イベント
        /// </summary>
        public static event EventHandler<TaskStateChangedEventArgs> TaskStateChanged;

        private static List<TaskInfo> _taskList = new List<TaskInfo>();

        /// <summary>
        /// 現在のタスク一覧を取得します。
        /// </summary>
        /// <returns>現在のタスク一覧</returns>
        public static IList<TaskInfo> GetTaskList()
        {
            return _taskList;
        }

        /// <summary>
        /// 新しいタスクを作成します。
        /// </summary>
        /// <typeparam name="TParameter">パラメータの型</typeparam>
        /// <param name="sender">呼出元</param>
        /// <param name="processType">プロセスの型</param>
        /// <param name="param">パラメータ</param>
        /// <returns>タスク</returns>
        internal static TaskInfo CreateNewTask<TParameter>(object sender, Type processType, TParameter param) where TParameter : IEntity
        {
            TaskInfo task = new TaskInfo(sender, processType, param);

            _taskList.Add(task);

            onTaskStateChanged(new TaskStateChangedEventArgs(task));

            setupTaskDelegate(task);

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
            TaskInfo task = new TaskInfo(sender, processType);

            _taskList.Add(task);

            onTaskStateChanged(new TaskStateChangedEventArgs(task));

            setupTaskDelegate(task);

            return task;
        }

        // タスクのデリゲートをセットします。
        private static void setupTaskDelegate(TaskInfo task)
        {
            task.TaskStateChanged += new EventHandler<TaskStateChangedEventArgs>(task_TaskStateChanged);
        }

        // タスク タスク状態変更イベント
        private static void task_TaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            if (e.Task.IsEnd || e.Task.IsCancel)
            {
                // 終了または、キャンセルされた場合
                _taskList.Remove(e.Task);
            }

            onTaskStateChanged(new TaskStateChangedEventArgs(e.Task));
        }

        // タスク変更イベントを発生させます。
        private static void onTaskStateChanged(TaskStateChangedEventArgs e)
        {
            if (TaskStateChanged != null)
            {
                TaskStateChanged(null, e);
            }
        }

        #endregion
    }
}
