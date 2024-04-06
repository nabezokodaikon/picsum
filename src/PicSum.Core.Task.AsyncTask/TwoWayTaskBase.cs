using PicSum.Core.Task.Base;
using System;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスク基底クラス
    /// </summary>
    /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
    public abstract class TwoWayTaskBase<TCallbackEventArgs>
        : AbstractAsyncTask
        where TCallbackEventArgs : IEntity
    {
        /// <summary>
        /// コールバックイベント
        /// </summary>
        internal event AsyncTaskCallbackEventHandler<TCallbackEventArgs> Callback;

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// タスクをキャンセルします。
        /// </summary>
        /// <exception cref="TaskCancelException">タスクキャンセル例外</exception>
        public void Cancel()
        {
            throw new TaskCancelException(this.Task);
        }

        /// <summary>
        /// コールバックイベントを発生させます。
        /// </summary>
        /// <param name="e">コールバックイベント引数クラス</param>
        protected void OnCallback(TCallbackEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (this.Callback == null) throw new NullReferenceException(nameof(this.Callback));

            this.Callback(this.Task, e);
        }
    }

    /// <summary>
    /// タスク基底クラス
    /// </summary>
    /// <typeparam name="TParameter">パラメータの型</typeparam>
    /// <typeparam name="TCallbackEventArgs">コールバックイベント引数クラスの型</typeparam>
    public abstract class TwoWayTaskBase<TParameter, TCallbackEventArgs>
        : AbstractAsyncTask
        where TParameter : IEntity
        where TCallbackEventArgs : IEntity
    {
        /// <summary>
        /// コールバックイベント
        /// </summary>
        internal event AsyncTaskCallbackEventHandler<TCallbackEventArgs> Callback;

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="param">パラメータ</param>
        public abstract void Execute(TParameter param);

        /// <summary>
        /// タスクをキャンセルします。
        /// </summary>
        /// <exception cref="TaskCancelException">タスクキャンセル例外</exception>
        public void Cancel()
        {
            throw new TaskCancelException(this.Task);
        }

        /// <summary>
        /// コールバックイベントを発生させます。
        /// </summary>
        /// <param name="e">コールバックイベント引数クラス</param>
        protected void OnCallback(TCallbackEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (this.Callback == null) throw new NullReferenceException(nameof(this.Callback));

            this.Callback(this.Task, e);
        }
    }
}