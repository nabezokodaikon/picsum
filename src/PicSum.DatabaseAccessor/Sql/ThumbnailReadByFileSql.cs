using PicSum.DatabaseAccessor.Dto;
using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイルを指定してサムネイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ThumbnailReadByFileSql
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

        public ThumbnailReadByFileSql(string filePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
