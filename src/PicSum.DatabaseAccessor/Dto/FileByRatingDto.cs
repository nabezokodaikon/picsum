using SWF.Core.DatabaseAccessor;
using System.Data;

namespace PicSum.DatabaseAccessor.Dto
{
    public struct FileByRatingDto
        : IDto
    {
        public string FilePath { get; private set; }
        public DateTime RegistrationDate { get; private set; }

        public void Read(IDataReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            this.FilePath = (string)reader["file_path"];
            this.RegistrationDate = (DateTime)reader["registration_date"];
        }
    }
}
