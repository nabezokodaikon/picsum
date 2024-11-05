using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FilesGetByRatingLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public IList<FileByRatingDto> Execute(int rating)
        {
            var sql = new FileReadByRatingSql(rating);
            var dtoList = Instance<IFileInfoDB>.Value.ReadList<FileByRatingDto>(sql);

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
