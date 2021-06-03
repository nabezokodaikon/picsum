using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイルを指定してサムネイルを読込みます。
    /// </summary>
    public class ReadThumbnailByFileSql : SqlBase<ThumbnailDto>
    {
        public ReadThumbnailByFileSql(string filePath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
