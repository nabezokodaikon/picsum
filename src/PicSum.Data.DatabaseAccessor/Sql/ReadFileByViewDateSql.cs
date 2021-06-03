using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 表示日付を指定して、ファイルを読込みます。
    /// </summary>
    public class ReadFileByViewDateSql : SqlBase<SingleValueDto<string>>
    {
        public ReadFileByViewDateSql(DateTime viewDate)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("view_date", viewDate));
        }
    }
}
