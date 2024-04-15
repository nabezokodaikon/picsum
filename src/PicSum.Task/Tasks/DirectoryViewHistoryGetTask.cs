using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class DirectoryViewHistoryGetTask
        : AbstractTwoWayTask<ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute()
        {
            var logic = new FileShallowInfoGetLogic(this);
            var result = new ListResult<FileShallowInfoEntity>();

            foreach (var directoryPath in (new DirectoryViewHistoryGetLogic(this)).Execute())
            {
                this.CheckCancel();

                try
                {
                    result.Add(logic.Execute(directoryPath));
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new TaskException(this.ID, ex));
                    continue;
                }
            }

            this.Callback(result);
        }
    }
}
