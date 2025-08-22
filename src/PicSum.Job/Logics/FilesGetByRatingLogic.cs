using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>

    internal sealed class FilesGetByRatingLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public async ValueTask<FileByRatingDto[]> Execute(IDatabaseConnection con, int rating)
        {
            using (TimeMeasuring.Run(true, "FilesGetByRatingLogic.Execute"))
            {
                var sql = new FileReadByRatingSql(rating);
                var dtoList = await con.ReadList<FileByRatingDto>(sql).WithConfig();
                return dtoList;
            }
        }
    }
}
