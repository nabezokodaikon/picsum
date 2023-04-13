using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダを指定してフォルダ状態を取得します。
    /// </summary>
    public class ReadDirectoryStateByDirectorySql : SqlBase<DirectoryStateDto>
    {
        public ReadDirectoryStateByDirectorySql(string directoryPath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("directory_path", directoryPath));
        }
    }
}
