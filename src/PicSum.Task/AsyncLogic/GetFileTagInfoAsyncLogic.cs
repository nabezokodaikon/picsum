using System;
using System.Collections.Generic;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルタグ取得非同期ロジック
    /// </summary>
    public class GetFileTagInfoAsyncLogic : AbstractAsyncLogic
    {
        public GetFileTagInfoAsyncLogic(AbstractAsyncFacade facade) : base(facade) { }

        public ListEntity<FileTagInfoEntity> Execute(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            ReadFileTagSql sql = new ReadFileTagSql(filePathList);
            IList<FileTagDto> dtoList = DatabaseManager<FileInfoConnection>.ReadList<FileTagDto>(sql);

            ListEntity<FileTagInfoEntity> infoList = new ListEntity<FileTagInfoEntity>();
            foreach (FileTagDto dto in dtoList)
            {
                CheckCancel();

                FileTagInfoEntity info = new FileTagInfoEntity();
                info.Tag = dto.Tag;
                info.IsAll = dto.IsAll;
                infoList.Add(info);
            }

            return infoList;
        }
    }
}
