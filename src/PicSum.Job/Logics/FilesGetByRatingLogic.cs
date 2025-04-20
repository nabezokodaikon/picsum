using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FilesGetByRatingLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public FileByRatingDto[] Execute(int rating)
        {
            var sql = new FileReadByRatingSql(rating);
            var dtoList = Instance<IFileInfoDB>.Value.ReadList<FileByRatingDto>(sql);
            return dtoList
                .AsValueEnumerable()
                .Where(dto => FileUtil.CanAccess(dto.FilePath))
                .ToArray();
        }
    }
}
