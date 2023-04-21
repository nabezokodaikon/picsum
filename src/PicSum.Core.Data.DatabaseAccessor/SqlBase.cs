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
        private readonly string sqlText;

        public List<IDbDataParameter> ParameterList { get; protected set; }

        public SqlBase(string sqlText)
        {
            this.sqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));

            this.ParameterList = new List<IDbDataParameter>();
        }

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <returns>SQL文</returns>
        public string GetExecuteSql()
        {
            return SqlUtil.GetExecuteSql(this.sqlText, this.ParameterList);
        }
    }

    /// <summary>
    /// データアクセサ
    /// </summary>
    public abstract class SqlBase<TDto>
        where TDto : IDto
    {
        private readonly string sqlText;

        public List<IDbDataParameter> ParameterList { get; protected set; }

        public SqlBase(string sqlText)
        {
            this.sqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));

            this.ParameterList = new List<IDbDataParameter>();
        }

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <returns>SQL文</returns>
        public string GetExecuteSql()
        {
            return SqlUtil.GetExecuteSql(this.sqlText, this.ParameterList);
        }
    }
}
