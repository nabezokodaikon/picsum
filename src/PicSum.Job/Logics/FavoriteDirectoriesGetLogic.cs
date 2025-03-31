using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FavoriteDirectoriesGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public string[] Execute()
        {
            var sql = new FavoriteDirectoriesReadSql();
            var dtoList = Instance<IFileInfoDB>.Value.ReadList<SingleValueDto<string>>(sql);

            return [.. dtoList
                .Select(dto => dto.GetValueOrDefault(string.Empty))
                .Where(_ => !FileUtil.IsSystemRoot(_)
                            && !FileUtil.IsDrive(_)
                            && FileUtil.IsExistsDirectory(_))];
        }
    }
}
