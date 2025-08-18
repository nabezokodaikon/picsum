using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

    public sealed class ThumbnailExistsByFileSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
SELECT EXISTS (
  SELECT 1
    FROM t_thumbnail tt
   WHERE tt.file_path = :file_path
);
";

        public ThumbnailExistsByFileSql(string filePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.Parameters = [
                SqlUtil.CreateParameter("file_path", filePath)
            ];
        }
    }
}
