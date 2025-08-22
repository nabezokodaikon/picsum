using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SWF.Core.DatabaseAccessor
{

    public static partial class SqlUtil
    {
        private static readonly CompositeFormat NUMBERING_SQL_PARAMETER_FORMAT
            = CompositeFormat.Parse("{0}_{1}");

        // 番号付パラメータ名正規表現
        [GeneratedRegex("_\\d+$")]
        private static partial Regex NumberingParameterNameRegex();

        // 置換文字列正規表現
        [GeneratedRegex("{.*}")]
        private static partial Regex ReplaceRegex();

        // 置換パラメータ文字列正規表現
        [GeneratedRegex(":[a-z][a-z|_]+[a-z]")]
        private static partial Regex ParameterRegex();

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <param name="sqlText">SQL文字列</param>
        /// <param name="paramList">パラメータリスト</param>
        /// <returns>SQL</returns>
        public static string GetExecuteSql(string sqlText, IDbDataParameter[] paramList)
        {
            ArgumentException.ThrowIfNullOrEmpty(sqlText, nameof(sqlText));
            ArgumentNullException.ThrowIfNull(paramList, nameof(paramList));

            // SQLテキスト内の置換文字列を取得します。
            var oldText = SqlUtil.GetRepalceText(sqlText);

            if (string.IsNullOrEmpty(oldText))
            {
                // 置換文字列が存在しないので、SQL文字列を実行SQLとして返します。
                return sqlText;
            }
            else
            {
                // パラメータリスト内の、番号付パラメータの個数を取得します。
                var count = SqlUtil.GetNumberingParameterCount(paramList);

                // 置換文字列内のパラメータ文字列リストを取得します。
                var paramStringList = SqlUtil.GetParameterNameList(oldText);

                var newText = new StringBuilder();

                newText.Append('(');

                for (var i = 0; i < count; i++)
                {
                    newText.Append('(');

                    var text = oldText.Replace("{", "", StringComparison.Ordinal).Replace("}", "", StringComparison.Ordinal);

                    foreach (var paramString in paramStringList)
                    {
                        var r = new Regex($"{paramString}\\s|{paramString}$");
                        text = r.Replace(text, string.Format(null, NUMBERING_SQL_PARAMETER_FORMAT, paramString, i.ToString(CultureInfo.InvariantCulture)));
                    }

                    newText.Append(text);

                    newText.Append(')');

                    if (i < count - 1)
                    {
                        newText.Append(" OR ");
                    }
                }

                newText.Append(')');

                return sqlText.Replace(oldText, newText.ToString(), StringComparison.Ordinal);
            }
        }

        // パラメータリスト内の、番号付パラメータの個数を取得します。
        private static int GetNumberingParameterCount(IDbDataParameter[] paramList)
        {
            var dic = new Dictionary<string, int>();

            foreach (var param in paramList)
            {
                if (SqlUtil.NumberingParameterNameRegex().IsMatch(param.ParameterName))
                {
                    var paramterName = SqlUtil.NumberingParameterNameRegex().Replace(param.ParameterName, "");
                    if (!dic.TryAdd(paramterName, 1))
                    {
                        dic[paramterName] += 1;
                    }
                }
            }

            if (dic.Count == 0)
            {
                throw new InvalidOperationException("番号付パラメータが存在しません。");
            }

            return dic.Values.First();
        }

        // SQLテキスト内の置換文字列を取得します。
        private static string GetRepalceText(string sqlText)
        {
            var m = SqlUtil.ReplaceRegex().Match(sqlText);

            if (m.Success)
            {
                return m.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        // 置換文字列内のパラメータ名リストを取得します。
        private static string[] GetParameterNameList(string replaceText)
        {
            var list = new List<string>();

            var m = SqlUtil.ParameterRegex().Match(replaceText);

            while (m.Success)
            {
                list.Add(m.Value);
                m = m.NextMatch();
            }

            return [.. list];
        }

        public static IDbDataParameter CreateParameter(string paramName, string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

            return CreateParameter(paramName, value, DbType.String);
        }

        public static IDbDataParameter CreateParameter(string paramName, string value, int size)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

            return CreateParameter(paramName, value, size, DbType.String);
        }

        public static IDbDataParameter CreateParameter(string paramName, int value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

            return CreateParameter(paramName, value, DbType.Int32);
        }

        public static IDbDataParameter CreateParameter(string paramName, long value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

            return CreateParameter(paramName, value, DbType.Int64);
        }

        public static IDbDataParameter CreateParameter(string paramName, bool value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

            return CreateParameter(paramName, value, DbType.Boolean);
        }

        public static IDbDataParameter CreateParameter(string paramName, DateTime value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

            return CreateParameter(paramName, value, DbType.DateTime);
        }

        public static IDbDataParameter CreateParameter(string paramName, byte[] value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            return CreateParameter(paramName, value, DbType.Binary);
        }

        public static IDbDataParameter CreateParameter(string paramName, DBNull value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

            return new SQLiteParameter(paramName, value);
        }

        public static IDbDataParameter[] CreateParameter(string paramName, string[] valueList)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(valueList, nameof(valueList));

            var list = new List<IDbDataParameter>();

            for (var i = 0; i < valueList.Length; i++)
            {
                var param = string.Format(null, NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString(CultureInfo.InvariantCulture));
                var value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.String));
            }

            return [.. list];
        }

        public static IDbDataParameter[] CreateParameter(string paramName, string[] valueList, int size)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(valueList, nameof(valueList));

            var list = new List<IDbDataParameter>();

            for (var i = 0; i < valueList.Length; i++)
            {
                var param = string.Format(null, NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString(CultureInfo.InvariantCulture));
                var value = valueList[i];
                list.Add(CreateParameter(param, value, size, DbType.String));
            }

            return [.. list];
        }

        public static IDbDataParameter[] CreateParameter(string paramName, int[] valueList)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(valueList, nameof(valueList));

            var list = new List<IDbDataParameter>();

            for (var i = 0; i < valueList.Length; i++)
            {
                var param = string.Format(null, NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString(CultureInfo.InvariantCulture));
                var value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.Int32));
            }

            return [.. list];
        }

        public static IDbDataParameter[] CreateParameter(string paramName, bool[] valueList)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(valueList, nameof(valueList));

            var list = new List<IDbDataParameter>();

            for (var i = 0; i < valueList.Length; i++)
            {
                var param = string.Format(null, NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString(CultureInfo.InvariantCulture));
                var value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.Boolean));
            }

            return [.. list];
        }

        public static IDbDataParameter[] CreateParameter(string paramName, DateTime[] valueList)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(valueList, nameof(valueList));

            var list = new List<IDbDataParameter>();

            for (var i = 0; i < valueList.Length; i++)
            {
                var param = string.Format(null, NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString(CultureInfo.InvariantCulture));
                var value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.DateTime));
            }

            return [.. list];
        }

        public static IDbDataParameter[] CreateParameter(string paramName, byte[][] valueList)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(valueList, nameof(valueList));

            var list = new List<IDbDataParameter>();

            for (var i = 0; i < valueList.Length; i++)
            {
                var param = string.Format(null, NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString(CultureInfo.InvariantCulture));
                var value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.Binary));
            }

            return [.. list];
        }

        private static SQLiteParameter CreateParameter(string paramName, object value, DbType dbType)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            var param = new SQLiteParameter
            {
                ParameterName = paramName,
                Value = value,
                DbType = dbType
            };

            return param;
        }

        private static SQLiteParameter CreateParameter(string paramName, object value, int size, DbType dbType)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            var param = new SQLiteParameter
            {
                ParameterName = paramName,
                Value = value,
                Size = size,
                DbType = dbType
            };

            return param;
        }
    }
}
