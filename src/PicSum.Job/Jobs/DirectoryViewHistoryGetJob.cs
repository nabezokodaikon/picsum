using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>

    public sealed class DirectoryViewHistoryGetJob
        : AbstractTwoWayJob<ListResult<FileShallowInfoEntity>>
    {
        protected override ValueTask Execute()
        {
            var logic = new FileShallowInfoGetLogic(this);
            var result = new ListResult<FileShallowInfoEntity>();

            foreach (var directoryPath in this.GetHistories())
            {
                this.ThrowIfJobCancellationRequested();

                try
                {
                    var info = logic.Get(directoryPath, false);
                    if (!info.IsEmpty)
                    {
                        result.Add(info);
                    }
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(ex);
                    continue;
                }
            }

            this.Callback(result);

            return ValueTask.CompletedTask;
        }

        private string[] GetHistories()
        {
            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var logic = new DirectoryViewHistoryGetLogic(this);
                return logic.Execute(con);
            }
        }
    }
}
