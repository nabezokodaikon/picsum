using System;
using System.Collections.Generic;
using System.Data;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// SQL基底クラス
    /// </summary>
    public abstract class SqlBase
    {
        public string SqlText { get; private set; }
        public List<IDbDataParameter> ParameterList { get; protected set; }

        public SqlBase(string sqlText)
        {
            this.SqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));
            this.ParameterList = new List<IDbDataParameter>();
        }

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <returns>SQL文</returns>
        public string GetExecuteSql()
        {
            return SqlUtil.GetExecuteSql(this.SqlText, this.ParameterList);
        }
    }

    /// <summary>
    /// データアクセサ
    /// </summary>
    public abstract class SqlBase<TDto> where TDto : IDto
    {
        public string SqlText { get; private set; }
        public List<IDbDataParameter> ParameterList { get; protected set; }

        public SqlBase(string sqlText)
        {
            this.SqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));
            this.ParameterList = new List<IDbDataParameter>();
        }

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <returns>SQL文</returns>
        public string GetExecuteSql()
        {
            return SqlUtil.GetExecuteSql(this.SqlText, this.ParameterList);
        }
    }
}
