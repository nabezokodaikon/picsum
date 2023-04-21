using PicSum.Core.Base.Conf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// SQLユーティリティ
    /// </summary>
    internal static class SqlUtil
    {
        // 番号付パラメータ名正規表現
        private static readonly Regex NUMBERING_PARAMETER_NAME_REGEX = new Regex("_\\d+$");

        // 置換文字列正規表現
        private static readonly Regex REPLACE_REGEX = new Regex("{.*}");

        // 置換パラメータ文字列正規表現
        private static readonly Regex PARAMETER_REGEX = new Regex(":[a-z][a-z|_]+[a-z]");

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <param name="sqlText">SQL文字列</param>
        /// <param name="paramList">パラメータリスト</param>
        /// <returns>SQL</returns>
        public static string GetExecuteSql(string sqlText, IList<IDbDataParameter> paramList)
        {
            if (sqlText == null) throw new ArgumentNullException(nameof(sqlText));
            if (paramList == null) throw new ArgumentNullException(nameof(paramList));

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

                newText.Append("(");

                for (int i = 0; i < count; i++)
                {
                    newText.Append("(");

                    var text = oldText.Replace("{", "").Replace("}", "");

                    foreach (var paramString in paramStringList)
                    {
                        var r = new Regex(string.Format("{0}\\s|{0}$", paramString));
                        text = r.Replace(text, string.Format(ApplicationConst.NUMBERING_SQL_PARAMETER_FORMAT + " ", paramString, i.ToString()));
                    }

                    newText.Append(text);

                    newText.Append(")");

                    if (i < count - 1)
                    {
                        newText.Append(" OR ");
                    }
                }

                newText.Append(")");

                return sqlText.Replace(oldText, newText.ToString());
            }
        }

        private static string ToSqlFileName(string sqlName)
        {
            return sqlName.Substring(0, sqlName.Length - 3);
        }

        // パラメータリスト内の、番号付パラメータの個数を取得します。
        private static int GetNumberingParameterCount(IList<IDbDataParameter> paramList)
        {
            var dic = new Dictionary<string, int>();

            foreach (var param in paramList)
            {
                if (SqlUtil.NUMBERING_PARAMETER_NAME_REGEX.IsMatch(param.ParameterName))
                {
                    var paramterName = SqlUtil.NUMBERING_PARAMETER_NAME_REGEX.Replace(param.ParameterName, "");
                    if (!dic.ContainsKey(paramterName))
                    {
                        dic.Add(paramterName, 1);
                    }
                    else
                    {
                        dic[paramterName] += 1;
                    }
                }
            }

            if (dic.Count == 0)
            {
                throw new ArgumentException("番号付パラメータが存在しません。", "paramList");
            }

            return dic.Values.First();
        }

        // SQLテキスト内の置換文字列を取得します。
        private static string GetRepalceText(string sqlText)
        {
            var m = SqlUtil.REPLACE_REGEX.Match(sqlText);

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
        private static IList<string> GetParameterNameList(string replaceText)
        {
            var list = new List<string>();

            var m = SqlUtil.PARAMETER_REGEX.Match(replaceText);

            while (m.Success)
            {
                list.Add(m.Value);
                m = m.NextMatch();
            }

            return list;
        }
    }
}
