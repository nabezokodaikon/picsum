using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイルを指定してサムネイルを読込みます。
    /// </summary>
    public sealed class ReadThumbnailByFileSql
        : SqlBase<ThumbnailDto>
    {
        private const string SQL_TEXT =
@"
SELECT tt.file_path
      ,tt.thumbnail_id
      ,tt.thumbnail_start_point
      ,tt.thumbnail_size
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
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
