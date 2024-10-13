using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルタグ取得非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FilesTagInfoGetLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public ListEntity<FileTagInfoEntity> Execute(IList<string> filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            var sql = new FileTagReadSql(filePathList);
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<FileTagDto>(sql);

            var infoList = new ListEntity<FileTagInfoEntity>();
            foreach (var dto in dtoList)
            {
                this.CheckCancel();

                var info = new FileTagInfoEntity
                {
                    Tag = dto.Tag,
                    IsAll = dto.IsAll
                };
                infoList.Add(info);
            }

            return infoList;
        }
    }
}
