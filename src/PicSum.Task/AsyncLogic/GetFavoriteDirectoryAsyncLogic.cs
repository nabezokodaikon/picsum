using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncLogic
{
    [SupportedOSPlatform("windows")]
    internal sealed class GetFavoriteDirectoryAsyncLogic
        : AbstractAsyncLogic
    {
        public GetFavoriteDirectoryAsyncLogic(AbstractAsyncFacade facade)
            : base(facade)
        {

        }

        public IList<string> Execute()
        {
            var sql = new ReadFavoriteDirectorySql();
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<SingleValueDto<string>>(sql);

            var dirList = dtoList
                .Where(dto => !string.IsNullOrEmpty(dto.Value) && FileUtil.CanAccess(dto.Value) && FileUtil.HasImageFile(dto.Value))
                .Select(dto => dto.Value)
                .ToList();

            return dirList;
        }
    }
}
