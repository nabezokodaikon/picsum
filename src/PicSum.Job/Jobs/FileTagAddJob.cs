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
    internal sealed class FileTagAddJob
        : AbstractOneWayJob<FileTagUpdateParameter>
    {
        protected override Task Execute(FileTagUpdateParameter param)
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
                var addTag = new FileTagAddLogic(this);
                var addFileMaster = new FileMasterAddLogic(this);
                var updateFileMaster = new FileMastercUpdateLogic(this);
                var registrationDate = DateTime.Now;

                foreach (var filePath in param.FilePathList)
                {
                    if (!updateTag.Execute(con, filePath, param.Tag, registrationDate))
                    {
                        if (!addTag.Execute(con, filePath, param.Tag, registrationDate))
                        {
                            if (!updateFileMaster.Execute(con, filePath))
                            {
                                addFileMaster.Execute(con, filePath);
                            }

                            addTag.Execute(con, filePath, param.Tag, registrationDate);
                        }
                    }
                }

                con.Commit();
            }

            return Task.CompletedTask;
        }
    }
}
