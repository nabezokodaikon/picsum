using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class FavoriteDirectoriesGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public IList<string> Execute()
        {
            var sql = new FavoriteDirectoriesReadSql();
            var dtoList = Dao<IFileInfoDB>.Instance.ReadList<SingleValueDto<string>>(sql);

            return [.. dtoList
                .Where(dto => !FileUtil.IsSystemRoot(dto.Value) && FileUtil.CanAccess(dto.Value))
                .Select(dto => dto.Value)];
        }
    }
}
