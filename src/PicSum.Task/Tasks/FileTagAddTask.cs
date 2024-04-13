using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// ファイルにタグを追加します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FileTagAddTask
        : AbstractOneWayTask<UpdateFileTagParameter>
    {
        protected override void Execute(UpdateFileTagParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var updateTag = new FileTagUpdateLogic(this);
                var addTag = new FileTagAddLogic(this);
                var addFileMaster = new FileMasterAddLogic(this);
                var updateFileMaster = new FileMastercUpdateLogic(this);
                var registrationDate = DateTime.Now;

                foreach (var filePath in param.FilePathList)
                {
                    if (!updateTag.Execute(filePath, param.Tag, registrationDate))
                    {
                        if (!addTag.Execute(filePath, param.Tag, registrationDate))
                        {
                            if (!updateFileMaster.Execute(filePath))
                            {
                                addFileMaster.Execute(filePath);
                            }

                            addTag.Execute(filePath, param.Tag, registrationDate);
                        }
                    }
                }

                tran.Commit();
            }
        }
    }
}
