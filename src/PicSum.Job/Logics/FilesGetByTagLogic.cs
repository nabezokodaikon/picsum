using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FilesGetByTagLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public IList<FileByTagDto> Execute(string tag)
        {
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new FileReadByTagSql(tag);
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<FileByTagDto>(sql);

            var list = new List<FileByTagDto>();
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
