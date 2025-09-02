using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>

    internal sealed class FilesGetByTagLogic(IJob job)
        : AbstractLogic(job)
    {
        public FileByTagDto[] Execute(IConnection con, string tag)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            using (TimeMeasuring.Run(true, "FilesGetByTagLogic.Execute"))
            {
                var sql = new FileReadByTagSql(tag);
                var dtoList = con.ReadList<FileByTagDto>(sql);
                return dtoList;
            }
        }
    }
}
