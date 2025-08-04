using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FilesGetByRatingLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public FileByRatingDto[] Execute(IDatabaseConnection con, int rating)
        {
            using (TimeMeasuring.Run(true, "FilesGetByRatingLogic.Execute"))
            {
                var sql = new FileReadByRatingSql(rating);
                var dtoList = con.ReadList<FileByRatingDto>(sql);
                return dtoList;
            }
        }
    }
}
