using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルにタグを追加します。
    /// </summary>

    internal sealed class FileTagUpdateJob
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

            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var updateTag = new FileTagUpdateLogic(this);
                var addFileMaster = new FileMasterAddLogic(this);
                var addDate = DateTime.Now;

                foreach (var filePath in param.FilePathList)
                {
                    if (!updateTag.Execute(con, filePath, param.Tag, addDate))
                    {
                        addFileMaster.Execute(con, filePath);
                        updateTag.Execute(con, filePath, param.Tag, addDate);
                    }
                }

                con.Commit();
            }

            return ValueTask.CompletedTask;
        }
    }
}
