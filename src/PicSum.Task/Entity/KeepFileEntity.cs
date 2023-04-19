using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSum.Task.Entity
{
    public sealed class KeepFileEntity
    {
        public string FilePath { get; private set; }
        public DateTime RegistrationDate { get; private set; }

        public KeepFileEntity(string filePath, DateTime registrationDate)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            this.FilePath = filePath;
            this.RegistrationDate = registrationDate;
        }

        internal KeepFileEntity(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            this.FilePath = filePath;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as KeepFileEntity;
            if (this.FilePath != other.FilePath)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.FilePath.GetHashCode();
        }
    }
}
