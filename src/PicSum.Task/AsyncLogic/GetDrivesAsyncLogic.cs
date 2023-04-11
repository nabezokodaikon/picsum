using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ドライブを取得します。
    /// </summary>
    internal class GetDrivesAsyncLogic : AsyncLogicBase
    {
        public GetDrivesAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<string> Execute()
        {
            List<string> list = new List<string>();
            foreach (string drive in FileUtil.GetDriveList())
            {
                CheckCancel();
                if (FileUtil.IsExists(drive))
                {                    
                    list.Add(drive);
                }
            }

            return list;
        }
    }
}
