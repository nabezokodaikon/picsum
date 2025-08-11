using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルにタグを追加します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileTagUpdateJob
        : AbstractOneWayJob<FileTagUpdateParameter>
    {
        protected async override ValueTask Execute(FileTagUpdateParameter param)
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
                    if (!updateTag.Execute(con, filePath, param.Tag, addDate))
                    {
                        addFileMaster.Execute(con, filePath);
                        updateTag.Execute(con, filePath, param.Tag, addDate);
                    }
                }

                con.Commit();
            }
        }
    }
}
