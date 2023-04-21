using PicSum.Core.Task.Base;
using System;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AsyncLogicBase
        : LogicBase
    {
        // ファサード
        private readonly AsyncFacadeBase facade;

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
        public AsyncLogicBase(AsyncFacadeBase facade)
        {
            this.facade = facade ?? throw new ArgumentNullException("facade");
        }
    }
}
