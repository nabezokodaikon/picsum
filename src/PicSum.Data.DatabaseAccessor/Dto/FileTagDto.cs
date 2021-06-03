using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    /// <summary>
    /// 複数ファイル情報DTO
    /// </summary>
    public class FileTagDto : IDto
    {
        private string _tag;
        private bool _isAll;

        public string Tag
        {
            get
            {
                return _tag;
            }
        }

        public bool IsAll
        {
            get
            {
                return _isAll;
            }
        }

        public void Read(IDataReader reader)
        {
            _tag = (string)reader["tag"];
            _isAll = bool.Parse(reader["is_all"].ToString());
        }
    }
}
