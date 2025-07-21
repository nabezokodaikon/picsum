using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FavoriteDirectoriesGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public FavoriteDirecotryDto[] Execute(IDatabaseConnection con)
        {
            using (TimeMeasuring.Run(true, "FavoriteDirectoriesGetLogic.Execute"))
            {
                var sql = new FavoriteDirectoriesReadSql();
                var dtoList = con.ReadList<FavoriteDirecotryDto>(sql);

                return [
                    .. dtoList
                    .AsEnumerable()
                    .Where(dto =>
                        !FileUtil.IsSystemRoot(dto.DirectoryPath)
                        && !FileUtil.IsExistsDrive(dto.DirectoryPath)
                        && FileUtil.IsExistsDirectory(dto.DirectoryPath))
                    .OrderBy(dto => dto.DirectoryPath)
                    .OrderByDescending(dto => dto.ViewCount)
                        ];
            }
        }
    }
}
