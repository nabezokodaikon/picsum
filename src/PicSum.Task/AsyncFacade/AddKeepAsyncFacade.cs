using System;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// キープリストにファイルを追加します。
    /// </summary>
    public class AddKeepAsyncFacade
        : OneWayFacadeBase<ListEntity<KeepFileEntity>>
    {
        public override void Execute(ListEntity<KeepFileEntity> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            OperatingKeepListAsyncLogic logic = new OperatingKeepListAsyncLogic(this);
            logic.AddKeep(param);
        }
    }
}
