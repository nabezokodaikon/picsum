﻿using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Dto
{
    public sealed class BookmarkDto
        : IDto
    {
        public string FilePath { get; private set; }
        public DateTime RegistrationDate { get; private set; }

        public void Read(IDataReader reader)
        {
            this.FilePath = (string)reader["file_path"];
            this.RegistrationDate = (DateTime)reader["registration_date"];
        }
    }
}