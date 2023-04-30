using PicSum.Core.Task.Base;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// ファサード基底クラス
    /// </summary>
    public abstract class OneWayFacadeBase 
        : AbstractAsyncFacade
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        public abstract void Execute();
    }

    /// <summary>
    /// ファサード基底クラス
    /// </summary>
    /// <typeparam name="TParameter">パラメータの型</typeparam>
    public abstract class OneWayFacadeBase<TParameter> : AbstractAsyncFacade
        where TParameter : IEntity
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="param">パラメータ</param>
        public abstract void Execute(TParameter param);
    }
}
