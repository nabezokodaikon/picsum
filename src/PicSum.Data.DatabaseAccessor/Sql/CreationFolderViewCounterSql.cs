using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class CreationFolderViewCounterSql
        : SqlBase
    {
        public CreationFolderViewCounterSql(string folderPath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", folderPath));
        }
    }
}
