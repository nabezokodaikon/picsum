using System;
using PicSum.Core.Task.Base;

namespace PicSum.Core.Task.AsyncTask
{
    /// <summary>
    /// 非同期ロジック基底クラス
    /// </summary>
    public abstract class AsyncLogicBase : LogicBase
    {
        // ファサード
        private readonly AsyncFacadeBase _facade;

        /// <summary>
        /// タスクがキャンセルされていないか確認します。
        /// </summary>
        /// <exception cref="TaskCancelException">タスクキャンセル例外</exception>
        protected void CheckCancel()
        {
            _facade.CheckCancel();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="facade">ファサード</param>
        public AsyncLogicBase(AsyncFacadeBase facade)
        {
            if (facade == null)
            {
                throw new ArgumentNullException("facade");
            }

            _facade = facade;
        }
    }
}
