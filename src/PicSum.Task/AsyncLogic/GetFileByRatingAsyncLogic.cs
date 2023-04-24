using System.Collections.Generic;
using PicSum.Core.Data.DatabaseAccessor;
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
    internal class GetFileByRatingAsyncLogic : AsyncLogicBase
    {
        public GetFileByRatingAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<FileByRatingDto> Execute(int rating)
        {
            ReadFileByRatingSql sql = new ReadFileByRatingSql(rating);
            IList<FileByRatingDto> dtoList = DatabaseManager<FileInfoConnection>.ReadList<FileByRatingDto>(sql);

            List<FileByRatingDto> list = new List<FileByRatingDto>();
            foreach (FileByRatingDto dto in dtoList)
            {
                CheckCancel();
                if (FileUtil.CanAccess(dto.FilePath))
                {                    
                    list.Add(dto);
                }
            }

            return list;
        }
    }
}
