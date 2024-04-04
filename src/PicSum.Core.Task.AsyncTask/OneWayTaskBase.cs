using PicSum.Core.Task.Base;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// タスク基底クラス
    /// </summary>
    public abstract class OneWayTaskBase
        : AbstractAsyncTask
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        public abstract void Execute();
    }

    /// <summary>
    /// タスク基底クラス
    /// </summary>
    /// <typeparam name="TParameter">パラメータの型</typeparam>
    public abstract class OneWayTaskBase<TParameter> : AbstractAsyncTask
        where TParameter : IEntity
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="param">パラメータ</param>
        public abstract void Execute(TParameter param);
    }
}
