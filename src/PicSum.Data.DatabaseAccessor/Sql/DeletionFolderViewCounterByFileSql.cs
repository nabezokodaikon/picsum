using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class DeletionFolderViewCounterByFileSql
        : SqlBase
    {
        public DeletionFolderViewCounterByFileSql(string folderPath) 
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", folderPath));
        }
    }
}