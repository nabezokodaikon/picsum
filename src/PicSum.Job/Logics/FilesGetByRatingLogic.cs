using PicSum.Core.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FilesGetByRatingLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public IList<FileByRatingDto> Execute(int rating)
        {
            var sql = new FileReadByRatingSql(rating);
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
