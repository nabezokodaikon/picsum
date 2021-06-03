using System;
using System.ComponentModel;
using PicSum.Core.Task.Base;
using PicSum.Core.Base.Exception;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// OneWayプロセスクラス
    /// </summary>
    /// <typeparam name="TFacade">ファサードの型</typeparam>
    public sealed class OneWayProcess<TFacade> : ProcessBase
        where TFacade : OneWayFacadeBase, new()
    {
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
    }
}
