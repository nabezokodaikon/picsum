using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ状態テーブルに登録します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryStateAddLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(DirectoryStateParameter directoryState)
        {

            if (string.IsNullOrEmpty(directoryState.DirectoryPath))
            {
                throw new ArgumentException("フォルダパスがNULLです。", nameof(directoryState));
            }

            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                throw new ArgumentException("選択ファイルパスがNULLです。", nameof(directoryState));
            }

            DirectoryStateCreationSql sql;
            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                sql = new DirectoryStateCreationSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending);
            }
            else
            {
                sql = new DirectoryStateCreationSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending, directoryState.SelectedFilePath);
            }

            return Instance<IFileInfoDB>.Value.Update(sql);
        }
    }
}
