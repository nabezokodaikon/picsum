using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class DeletionDirectoryViewCounterByFileSql
        : SqlBase
    {
        public DeletionDirectoryViewCounterByFileSql(string directoryPath) 
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", directoryPath));
        }
    }
}