using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetDirectoryViewHistoryAsyncTask
        : TwoWayTaskBase<ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute()
        {
            var logic = new GetFileShallowInfoAsyncLogic(this);
            var result = new ListEntity<FileShallowInfoEntity>();

            foreach (var directoryPath in (new GetDirectoryViewHistoryAsyncLogic(this)).Execute())
            {
                this.CheckCancel();

                try
                {
                    result.Add(logic.Execute(directoryPath));
                }
                catch (FileUtilException)
                {
                    continue;
                }
            }

            this.OnCallback(result);
        }
    }
}
