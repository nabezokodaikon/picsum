using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルのタグを削除します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal class FileTagDeleteJob
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

            using (var tran = Instance<IFileInfoDB>.Value.BeginTransaction())
            {
                var logic = new FileTagDeleteLogic(this);

                foreach (var filePath in param.FilePathList)
                {
                    logic.Execute(filePath, param.Tag);
                }

                tran.Commit();
            }

            return Task.CompletedTask;
        }
    }
}
