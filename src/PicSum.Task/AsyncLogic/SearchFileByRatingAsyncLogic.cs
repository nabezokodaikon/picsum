using System.Collections.Generic;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    internal class SearchFileByRatingAsyncLogic : AsyncLogicBase
    {
        public SearchFileByRatingAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<string> Execute(int rating)
        {
            ReadFileByRatingSql sql = new ReadFileByRatingSql(rating);
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
