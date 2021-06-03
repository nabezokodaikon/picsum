﻿using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグT作成
    /// </summary>
    public class CreationTagSql : SqlBase
    {
        public CreationTagSql(string filePath, string tag)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[] 
                { SqlParameterUtil.CreateParameter("file_path", filePath), 
                  SqlParameterUtil.CreateParameter("tag", tag) });
        }
    }
}
