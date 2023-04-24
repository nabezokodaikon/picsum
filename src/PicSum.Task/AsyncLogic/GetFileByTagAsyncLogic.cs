using System;
using System.Collections.Generic;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    internal class GetFileByTagAsyncLogic : AsyncLogicBase
    {
        public GetFileByTagAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<FileByTagDto> Execute(string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            ReadFileByTagSql sql = new ReadFileByTagSql(tag);
            IList<FileByTagDto> dtoList = DatabaseManager<FileInfoConnection>.ReadList<FileByTagDto>(sql);

            List<FileByTagDto> list = new List<FileByTagDto>();
            foreach (FileByTagDto dto in dtoList)
            {
                CheckCancel();
                if (FileUtil.CanAccess(dto.FilePath))
                {                    
                    list.Add(dto);
                }
            }

            return list;
        }
    }
}
