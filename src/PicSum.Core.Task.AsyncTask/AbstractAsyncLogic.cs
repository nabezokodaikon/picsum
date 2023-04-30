using PicSum.Core.Task.Base;
using System;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AbstractAsyncLogic
        : ILogic
    {
        // ファサード
        private readonly AbstractAsyncFacade facade;

        /// <summary>
        /// タスクがキャンセルされていないか確認します。
        /// </summary>
        /// <exception cref="TaskCancelException">タスクキャンセル例外</exception>
        protected void CheckCancel()
        {
            this.facade.CheckCancel();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="facade">ファサード</param>
        public AbstractAsyncLogic(AbstractAsyncFacade facade)
        {
            this.facade = facade ?? throw new ArgumentNullException("facade");
        }
    }
}
