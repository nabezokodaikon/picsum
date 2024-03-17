using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class GetFileByRatingAsyncLogic
        : AbstractAsyncLogic
    {
        public GetFileByRatingAsyncLogic(AbstractAsyncFacade facade)
            : base(facade)
        {

        }

        public IList<FileByRatingDto> Execute(int rating)
        {
            var sql = new ReadFileByRatingSql(rating);
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<FileByRatingDto>(sql);

            var list = new List<FileByRatingDto>();
            foreach (var dto in dtoList)
            {
                this.CheckCancel();
                if (FileUtil.CanAccess(dto.FilePath))
                {
                    list.Add(dto);
                }
            }

            return list;
        }
    }
}
