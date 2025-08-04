using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FilesGetByTagLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public FileByTagDto[] Execute(IDatabaseConnection con, string tag)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new FileReadByTagSql(tag);
            var dtoList = con.ReadList<FileByTagDto>(sql);
            return dtoList;
        }
    }
}
