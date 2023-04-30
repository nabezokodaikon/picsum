﻿using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 評価T更新
    /// </summary>
    public sealed class UpdateRatingSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE t_rating
   SET rating = :rating
      ,registration_date = :registration_date
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
";

        public UpdateRatingSql(string filePath, int rating, DateTime registrationDate)
            : base(SQL_TEXT)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            base.ParameterList.AddRange(new IDbDataParameter[]
                {
                    SqlParameterUtil.CreateParameter("file_path", filePath),
                    SqlParameterUtil.CreateParameter("rating", rating),
                    SqlParameterUtil.CreateParameter("registration_date", registrationDate)
                });
        }
    }
}
