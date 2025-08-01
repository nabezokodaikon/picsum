using System.Data;
using System.Runtime.Versioning;

namespace SWF.Core.DatabaseAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract class SqlBase(string sqlText)
    {
        private readonly string _sqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));

        public IDbDataParameter[] Parameters { get; protected set; } = [];

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <returns>SQL文</returns>
        public string GetExecuteSql()
        {
            return SqlUtil.GetExecuteSql(this._sqlText, this.Parameters);
        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract class SqlBase<TDto>(string sqlText)
        where TDto : IDto
    {
        private readonly string sqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));

        public IDbDataParameter[] Parameters { get; protected set; } = [];

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <returns>SQL文</returns>
        public string GetExecuteSql()
        {
            return SqlUtil.GetExecuteSql(this.sqlText, this.Parameters);
        }
    }
}
