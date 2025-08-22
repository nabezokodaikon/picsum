using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    internal sealed class FileRatingGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {

        public async ValueTask<int> Execute(IDatabaseConnection con, string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            var sql = new FileInfoReadSql(filePath);
            var dto = await con.ReadLine<FileInfoDto>(sql).WithConfig();
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
