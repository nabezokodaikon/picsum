using System;
using System.Collections.Generic;
using System.Data;

namespace PicSum.Core.DatabaseAccessor
{
    /// <summary>
    /// SQL基底クラス
    /// </summary>
    public abstract class SqlBase(string sqlText)
    {
        private readonly string sqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));

        public List<IDbDataParameter> ParameterList { get; protected set; } = [];

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
    public abstract class SqlBase<TDto>(string sqlText)
        where TDto : IDto
    {
        private readonly string sqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));

        public List<IDbDataParameter> ParameterList { get; protected set; } = [];

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
