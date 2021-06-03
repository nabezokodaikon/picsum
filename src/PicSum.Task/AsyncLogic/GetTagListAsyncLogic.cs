using System.Collections.Generic;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.AsyncFacade;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// タグの一覧を取得します。
    /// </summary>
    internal class GetTagListAsyncLogic : AsyncLogicBase
    {
        public GetTagListAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<string> Execute()
        {
            ReadAllTagSql sql = new ReadAllTagSql();
            IList<SingleValueDto<string>> dtoList = DatabaseManager<FileInfoConnection>.ReadList<SingleValueDto<string>>(sql);

            List<string> tabList = new List<string>();
            foreach (SingleValueDto<string> dto in dtoList)
            {
                CheckCancel();
                tabList.Add(dto.Value);
            }

            return tabList;
        }
    }
}
