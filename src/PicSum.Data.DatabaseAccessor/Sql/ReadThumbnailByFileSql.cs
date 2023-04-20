using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイルを指定してサムネイルを読込みます。
    /// </summary>
    public class ReadThumbnailByFileSql : SqlBase<ThumbnailDto>
    {
        private const string SQL_TEXT =
@"
SELECT tt.file_path
      ,tt.thumbnail_buffer
      ,tt.thumbnail_width
      ,tt.thumbnail_height
      ,tt.source_width
      ,tt.source_height
      ,tt.file_update_date
  FROM t_thumbnail tt
 WHERE tt.file_path = :file_path
";

        public ReadThumbnailByFileSql(string filePath)
            : base(SQL_TEXT)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
