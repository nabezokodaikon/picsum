using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using SWF.Core.StringAccessor;
using ZLinq;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルタグ取得非同期ロジック
    /// </summary>

    internal sealed class FilesTagsGetLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask<ListEntity<FileTagInfoEntity>> Execute(IConnection con, string[] filePathList)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            using (Measuring.Time(false, "FilesTagsGetLogic.Execute"))
            {
                const int TAKE_COUNT = 100;

                var length = filePathList.Length;
                var dtoList = new List<SingleValueDto<string>>(length);
                var startIndex = 0;

                while (startIndex < length)
                {
                    var targets = filePathList
                        .AsValueEnumerable()
                        .Skip(startIndex)
                        .Take(TAKE_COUNT)
                        .ToArray();

                    var sql = new FileTagReadSql(targets);
                    dtoList.AddRange(await con.ReadList<SingleValueDto<string>>(sql).False());

                    startIndex += TAKE_COUNT;
                }

                return new ListEntity<FileTagInfoEntity>(dtoList
                    .AsValueEnumerable()
                    .GroupBy(static dto => dto.Value)
                    .Select(dtos =>
                    {
                        var tags = dtos
                            .AsValueEnumerable()
                            .Select(static dto => dto.Value)
                            .ToArray();

#pragma warning disable CS8601 // Null 参照代入の可能性があります。
                        return new FileTagInfoEntity()
                        {
                            Tag = tags.AsValueEnumerable().First(),
                            IsAll = tags.Length == length,
                        };
#pragma warning restore CS8601 // Null 参照代入の可能性があります。

                    })
                    .OrderBy(static ent => ent.Tag, NaturalStringComparer.WINDOWS)
                    .ToArray());
            }
        }
    }
}
