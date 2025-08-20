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
        protected override async ValueTask Execute(FileTagUpdateParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            if (param.Tag == null)
            {
                throw new ArgumentException("タグがNULLです。", nameof(param));
            }

            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().WithConfig())
            {
                var updateTag = new FileTagUpdateLogic(this);
                var addFileMaster = new FileMasterAddLogic(this);
                var addDate = DateTime.Now;

                foreach (var filePath in param.FilePathList)
                {
                    if (!await updateTag.Execute(con, filePath, param.Tag, addDate).WithConfig())
                    {
                        await addFileMaster.Execute(con, filePath).WithConfig();
                        await updateTag.Execute(con, filePath, param.Tag, addDate).WithConfig();
                    }
                }

                await con.Commit().WithConfig();
            }
        }
    }
}
