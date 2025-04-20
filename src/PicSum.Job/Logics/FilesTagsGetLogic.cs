using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルタグ取得非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FilesTagsGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public ListEntity<FileTagInfoEntity> Execute(string[] filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            var sql = new FileTagReadSql(filePathList);
            var dtoList = Instance<IFileInfoDB>.Value.ReadList<FileTagDto>(sql);
            return new ListEntity<FileTagInfoEntity>(dtoList
                .AsValueEnumerable()
                .OrderBy(dto => dto.Tag, NaturalStringComparer.Windows)
                .Select(dto => new FileTagInfoEntity() { Tag = dto.Tag, IsAll = dto.IsAll })
                .ToArray());
        }
    }
}
