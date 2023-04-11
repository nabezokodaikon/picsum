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
    internal class SearchFileByTagAsyncLogic : AsyncLogicBase
    {
        public SearchFileByTagAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<string> Execute(string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            ReadFileByTagSql sql = new ReadFileByTagSql(tag);
            IList<SingleValueDto<string>> dtoList = DatabaseManager<FileInfoConnection>.ReadList<SingleValueDto<string>>(sql);

            List<string> list = new List<string>();
            foreach (SingleValueDto<string> dto in dtoList)
            {
                CheckCancel();
                if (FileUtil.CanAccess(dto.Value))
                {                    
                    list.Add(dto.Value);
                }
            }

            return list;
        }
    }
}
