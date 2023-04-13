using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class CreationDirectoryViewCounterSql
        : SqlBase
    {
        public CreationDirectoryViewCounterSql(string directoryPath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", directoryPath));
        }
    }
}
