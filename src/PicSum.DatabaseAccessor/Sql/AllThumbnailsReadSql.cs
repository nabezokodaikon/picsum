using PicSum.DatabaseAccessor.Dto;
using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

    public sealed class AllThumbnailsReadSql
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
";

        public AllThumbnailsReadSql()
            : base(SQL_TEXT)
        {

        }
    }
}
