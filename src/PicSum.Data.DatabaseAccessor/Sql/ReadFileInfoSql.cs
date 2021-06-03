using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 単一ファイル情報を読込みます。
    /// </summary>
    public class ReadFileInfoSql : SqlBase<FileInfoDto>
    {
        public ReadFileInfoSql(string filePath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
