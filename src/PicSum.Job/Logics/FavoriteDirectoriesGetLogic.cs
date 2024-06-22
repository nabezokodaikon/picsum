using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class FavoriteDirectoriesGetLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public IList<string> Execute()
        {
            var sql = new FavoriteDirectoriesReadSql();
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<SingleValueDto<string>>(sql);

            var dirList = dtoList
                .Where(dto => !FileUtil.IsSystemRoot(dto.Value) && FileUtil.CanAccess(dto.Value) && FileUtil.HasImageFiles(dto.Value))
                .Select(dto => dto.Value)
                .ToList();

            return dirList;
        }
    }
}
