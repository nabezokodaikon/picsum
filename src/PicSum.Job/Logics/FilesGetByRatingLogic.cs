using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
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
            var dtoList = Dao<FileInfoDB>.Instance.ReadList<FileByRatingDto>(sql);

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
