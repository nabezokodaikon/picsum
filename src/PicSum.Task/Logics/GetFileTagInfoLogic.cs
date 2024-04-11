using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// ファイルタグ取得非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class GetFileTagInfoLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
        public ListEntity<FileTagInfoEntity> Execute(IList<string> filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            var sql = new ReadFileTagSql(filePathList);
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
