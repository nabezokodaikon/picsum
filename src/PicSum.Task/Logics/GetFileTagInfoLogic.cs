using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entities;
using System;
using System.Collections.Generic;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// ファイルタグ取得非同期ロジック
    /// </summary>
    internal sealed class GetFileTagInfoLogic
        : AbstractAsyncLogic
    {
        public GetFileTagInfoLogic(IAsyncTask task)
            : base(task)
        {

        }

        public ListEntity<FileTagInfoEntity> Execute(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException(nameof(filePathList));
            }

            var sql = new ReadFileTagSql(filePathList);
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<FileTagDto>(sql);

            var infoList = new ListEntity<FileTagInfoEntity>();
            foreach (var dto in dtoList)
            {
                this.CheckCancel();

                var info = new FileTagInfoEntity();
                info.Tag = dto.Tag;
                info.IsAll = dto.IsAll;
                infoList.Add(info);
            }

            return infoList;
        }
    }
}
