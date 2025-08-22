using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// タグを更新します。
    /// </summary>
    /// <remarks>タグの存在確認として使用します。</remarks>

    internal sealed class FileTagUpdateLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public async ValueTask<bool> Execute(
            IConnection con, string filePath, string tag, DateTime addDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new TagUpdateSql(filePath, tag, addDate);
            return await con.Update(sql).WithConfig();
        }
    }
}
