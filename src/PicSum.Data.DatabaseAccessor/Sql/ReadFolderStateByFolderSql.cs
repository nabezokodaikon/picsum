using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダを指定してフォルダ状態を取得します。
    /// </summary>
    public class ReadFolderStateByFolderSql : SqlBase<FolderStateDto>
    {
        public ReadFolderStateByFolderSql(string folderPath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("folder_path", folderPath));
        }
    }
}
