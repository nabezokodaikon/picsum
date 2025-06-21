using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    internal sealed class FileRatingGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        [SupportedOSPlatform("windows10.0.17763.0")]
        public int Execute(IConnection con, string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            var sql = new FileInfoReadSql(filePath);
            var dto = con.ReadLine<FileInfoDto>(sql);
            if (dto != null)
            {
                return dto.Rating;
            }
            else
            {
                return 0;
            }
        }
    }
}
