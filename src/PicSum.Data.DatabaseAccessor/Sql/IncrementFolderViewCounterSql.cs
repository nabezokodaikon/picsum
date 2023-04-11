using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class IncrementFolderViewCounterSql
        : SqlBase
    {
        public IncrementFolderViewCounterSql(string folderPath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", folderPath));
        }
    }
}
