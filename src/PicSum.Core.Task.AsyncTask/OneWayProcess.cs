using System;
using System.ComponentModel;
using PicSum.Core.Task.Base;
using PicSum.Core.Base.Exception;
using System.Threading;
using NLog;
using SWF.Common;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// OneWayプロセスクラス
    /// </summary>
    /// <typeparam name="TFacade">ファサードの型</typeparam>
    public sealed class OneWayProcess<TFacade> : ProcessBase
        where TFacade : OneWayFacadeBase, new()
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container">コンテナ</param>
        internal OneWayProcess(IContainer container) : base(container) { }

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

            var facadeName = facade.GetType().Name;
            Thread.CurrentThread.Name = facadeName;
            Logger.Debug("タスクID: {0} Facade: {1} を開始します。", task.TaskId, facadeName);

            facade.SetTask(task);

            try
            {
                facade.Execute();
            }
            catch (TaskCancelException)
            {
                Logger.Debug("タスクID: {0} Facade: {1} がキャンセルされました。", task.TaskId, facadeName);

                SendMessageThread(OnCancelEnd, task);
                return;
            }
            catch (Exception ex)
            {
                Logger.Debug("タスクID: {0} Facade: {1} で例外が発生しました。\n{2}", task.TaskId, facadeName, ExceptionUtil.CreateDetailsMessage(ex));

                SendMessageThread(OnErrorEnd, new object[] { task, ex });
                return;
            }
            finally
            {
                Logger.Debug("タスクID: {0} Facade: {1} が終了しました。", task.TaskId, facadeName);
            }

            SendMessageThread(OnSuccessEnd, task);
        }
    }

    /// <summary>
    /// OneWayプロセスクラス
    /// </summary>
    /// <typeparam name="TFacade">ファサードの型</typeparam>
    /// <typeparam name="TParameter">パラメータの型</typeparam>
    public sealed class OneWayProcess<TFacade, TParameter> : ProcessBase
        where TFacade : OneWayFacadeBase<TParameter>, new()
        where TParameter : IEntity
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container">コンテナ</param>
        internal OneWayProcess(IContainer container) : base(container) { }

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

            var facadeName = facade.GetType().Name;
            Thread.CurrentThread.Name = facadeName;
            Logger.Debug("タスクID: {0} Facade: {1} を開始します。", task.TaskId, facadeName);

            facade.SetTask(task);

            try
            {
                facade.Execute((TParameter)task.Parameter);
            }
            catch (TaskCancelException)
            {
                Logger.Debug("タスクID: {0} Facade: {1} がキャンセルされました。", task.TaskId, facadeName);

                SendMessageThread(OnCancelEnd, task);
                return;
            }
            catch (Exception ex)
            {
                Logger.Debug("タスクID: {0} Facade: {1} で例外が発生しました。\n{2}", task.TaskId, facadeName, ExceptionUtil.CreateDetailsMessage(ex));

                SendMessageThread(OnErrorEnd, new object[] { task, ex });
                return;
            }
            finally
            {
                Logger.Debug("タスクID: {0} Facade: {1} が終了しました。", task.TaskId, facadeName);
            }

            SendMessageThread(OnSuccessEnd, task);
        }
    }
}
