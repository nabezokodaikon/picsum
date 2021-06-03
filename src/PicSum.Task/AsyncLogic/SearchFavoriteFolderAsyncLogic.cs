using System.Collections.Generic;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    internal class SearchFavoriteFolderAsyncLogic : AsyncLogicBase
    {
        public SearchFavoriteFolderAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<string> Execute()
        {
            ReadFavoriteFolderSql sql = new ReadFavoriteFolderSql();
            IList<SingleValueDto<string>> dtoList = DatabaseManager<FileInfoConnection>.ReadList<SingleValueDto<string>>(sql);

            List<string> list = new List<string>();
            foreach (SingleValueDto<string> dto in dtoList)
            {
                CheckCancel();
                if (FileUtil.CanAccess(dto.Value))
                {
                    list.Add(dto.Value);
                }
            }

            return list;
        }
    }
}
