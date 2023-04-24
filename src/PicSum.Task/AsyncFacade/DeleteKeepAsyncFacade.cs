using System;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// キープリストからファイルを削除します。
    /// </summary>
    public class DeleteKeepAsyncFacade
        : OneWayFacadeBase<ListEntity<string>>
    {
        public override void Execute(ListEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            OperatingKeepListAsyncLogic logic = new OperatingKeepListAsyncLogic(this);
            logic.RemoveKeep(param);
        }
    }
}
