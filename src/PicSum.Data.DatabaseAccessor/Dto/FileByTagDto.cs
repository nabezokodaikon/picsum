using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    public sealed class FileByTagDto
        : IDto
    {
        public string FilePath { get; private set; }
        public string Tag { get; private set; }
        public DateTime RegistrationDate { get; private set; }

        public void Read(IDataReader reader)
        {
            this.FilePath = (string)reader["file_path"];
            this.Tag = (string)reader["tag"];
            this.RegistrationDate = (DateTime)reader["registration_date"];
        }
    }
}
