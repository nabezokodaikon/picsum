using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルのタグを削除します。
    /// </summary>

    internal sealed class FileTagDeleteJob
        : AbstractOneWayJob<FileTagUpdateParameter>
    {
        protected override ValueTask Execute(FileTagUpdateParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            if (param.Tag == null)
            {
                throw new ArgumentException("タグがNULLです。", nameof(param));
            }

            using (var con = Instance<IFileInfoDao>.Value.ConnectWithTransaction())
            {
                var logic = new FileTagDeleteLogic(this);

                foreach (var filePath in param.FilePathList)
                {
                    logic.Execute(con, filePath, param.Tag);
                }

                con.Commit();
            }

            return ValueTask.CompletedTask;
        }
    }
}
