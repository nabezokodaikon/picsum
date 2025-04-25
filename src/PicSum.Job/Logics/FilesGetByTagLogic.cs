using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FilesGetByTagLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public FileByTagDto[] Execute(string tag)
        {
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new FileReadByTagSql(tag);
            var dtoList = Instance<IFileInfoDB>.Value.ReadList<FileByTagDto>(sql);
            return dtoList
                .AsValueEnumerable()
                .Where(dto => FileUtil.CanAccess(dto.FilePath))
                .ToArray();
        }
    }
}
