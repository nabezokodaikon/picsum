using SWF.Core.Base;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace SWF.Core.DatabaseAccessor
{
    /// <summary>
    /// SQLユーティリティ
    /// </summary>
    internal static partial class SqlUtil
    {
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
        public static string GetExecuteSql(string sqlText, IList<IDbDataParameter> paramList)
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

                    var text = oldText.Replace("{", "").Replace("}", "");

                    foreach (var paramString in paramStringList)
                    {
                        var r = new Regex($"{paramString}\\s|{paramString}$");
                        text = r.Replace(text, string.Format(AppConstants.NUMBERING_SQL_PARAMETER_FORMAT + " ", paramString, i.ToString()));
                    }

                    newText.Append(text);

                    newText.Append(')');

                    if (i < count - 1)
                    {
                        newText.Append(" OR ");
                    }
                }

                newText.Append(')');

                return sqlText.Replace(oldText, newText.ToString());
            }
        }

        // パラメータリスト内の、番号付パラメータの個数を取得します。
        private static int GetNumberingParameterCount(IList<IDbDataParameter> paramList)
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
                throw new ArgumentException("番号付パラメータが存在しません。", nameof(paramList));
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
        private static List<string> GetParameterNameList(string replaceText)
        {
            var list = new List<string>();

            var m = SqlUtil.ParameterRegex().Match(replaceText);

            while (m.Success)
            {
                list.Add(m.Value);
                m = m.NextMatch();
            }

            return list;
        }
    }
}
