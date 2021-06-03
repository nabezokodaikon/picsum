using System.Collections.Generic;
using System.Data;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// SQL基底クラス
    /// </summary>
    public abstract class SqlBase
    {
        private List<IDbDataParameter> _parameterList = new List<IDbDataParameter>();

        /// <summary>
        /// パラメータリスト
        /// </summary>
        public List<IDbDataParameter> ParameterList
        {
            get
            {
                return _parameterList;
            }
        }

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <returns>SQL文</returns>
        public string GetExecuteSql()
        {
            // 自身のクラス名からSQL文を取得します。
            string sqlText = SqlManager.GetSqlText(this.GetType().Name);

            return SqlFileUtil.GetExecuteSql(sqlText, _parameterList);
        }
    }

    /// <summary>
    /// データアクセサ
    /// </summary>
    public abstract class SqlBase<TDto> where TDto : IDto
    {
        private List<IDbDataParameter> _parameterList = new List<IDbDataParameter>();

        /// <summary>
        /// パラメータリスト
        /// </summary>
        public List<IDbDataParameter> ParameterList
        {
            get
            {
                return _parameterList;
            }
        }

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <returns>SQL文</returns>
        public string GetExecuteSql()
        {
            // 自身のクラス名からSQL文を取得します。
            string sqlText = SqlManager.GetSqlText(this.GetType().Name);

            return SqlFileUtil.GetExecuteSql(sqlText, _parameterList);
        }
    }
}
