using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダを指定してフォルダ状態を取得します。
    /// </summary>
    public sealed class ReadDirectoryStateByDirectorySql
        : SqlBase<DirectoryStateDto>
    {
        const string SQL_TEXT =
@"
SELECT mf1.file_path AS directory_path
      ,tfs.sort_type_id
      ,tfs.is_ascending
      ,tfs.selected_file_path
  FROM m_file mf1
       INNER JOIN t_directory_state tfs
          ON tfs.file_id = mf1.file_id
 WHERE mf1.file_path = :directory_path
";

        public ReadDirectoryStateByDirectorySql(string directoryPath)
            : base(SQL_TEXT)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("directory_path", directoryPath));
        }
    }
}
