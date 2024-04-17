using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Paramters;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルのタグを削除します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class FileTagDeleteJob
        : AbstractOneWayJob<UpdateFileTagParameter>
    {
        protected override void Execute(UpdateFileTagParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            if (param.Tag == null)
            {
                throw new ArgumentException("タグがNULLです。", nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var logic = new FileTagDeleteLogic(this);

                foreach (var filePath in param.FilePathList)
                {
                    logic.Execute(filePath, param.Tag);
                }

                tran.Commit();
            }
        }
    }
}
