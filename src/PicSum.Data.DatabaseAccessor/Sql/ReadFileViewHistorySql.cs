using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイルの表示履歴を読込みます。
    /// </summary>
    public class ReadFileViewHistorySql : SqlBase<SingleValueDto<DateTime>>
    {
        public ReadFileViewHistorySql() : base() { }
    }
}
