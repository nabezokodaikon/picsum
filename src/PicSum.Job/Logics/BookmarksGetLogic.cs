using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class BookmarksGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public IList<BookmarkDto> Execute()
        {
            var sql = new BookmarksReadSql();
            var dtoList = Dao<FileInfoDB>.Instance.ReadList<BookmarkDto>(sql);

            var list = new List<BookmarkDto>();
            foreach (var dto in dtoList)
            {
                if (FileUtil.CanAccess(dto.FilePath))
                {
                    list.Add(dto);
                }
            }

            return list;
        }
    }
}
