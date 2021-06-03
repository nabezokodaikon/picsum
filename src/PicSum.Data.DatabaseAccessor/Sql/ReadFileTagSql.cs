using System.Collections.Generic;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 複数ファイルの情報を取得します。
    /// </summary>
    public class ReadFileTagSql : SqlBase<FileTagDto>
    {
        public ReadFileTagSql(IList<string> filePathList)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_count", filePathList.Count));
            base.ParameterList.AddRange(SqlParameterUtil.CreateParameter("file_path", filePathList));
        }
    }
}
