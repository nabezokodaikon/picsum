using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    internal sealed class FileRatingGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        [SupportedOSPlatform("windows10.0.17763.0")]
        public int Execute(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            var sql = new FileInfoReadSql(filePath);
            var dto = Instance<IFileInfoDB>.Value.ReadLine<FileInfoDto>(sql);
            if (dto != default(FileInfoDto))
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
