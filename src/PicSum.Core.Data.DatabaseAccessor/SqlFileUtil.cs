using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PicSum.Core.Base.Conf;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// SQLファイルユーティリティ
    /// </summary>
    internal static class SqlFileUtil
    {
        // SQLファイルエンコード
        private static readonly Encoding _encoding = Encoding.GetEncoding(932);

        // 番号付パラメータ名正規表現
        private static readonly Regex _numberingParameterNameRegex = new Regex("_\\d+$");

        // 置換文字列正規表現
        private static readonly Regex _replaceRegex = new Regex("{.*}");

        // 置換パラメータ文字列正規表現
        private static readonly Regex _parameterRegex = new Regex(":[a-z][a-z|_]+[a-z]");

        /// <summary>
        /// SQLファイルを読込みます。
        /// </summary>
        /// <param name="sqlFolder">SQLフォルダ</param>
        /// <param name="sqlName">データアクセサ名</param>
        /// <returns>SQLファイル内の文字列</returns>
        public static string ReadSqlFile(string sqlFolder, string sqlName)
        {
            string sqlFileName = toSqlFileName(sqlName);

            string path = string.Format("{0}\\{1}{2}", sqlFolder, sqlFileName, ApplicationConst.SqlFileExtension);

            return File.ReadAllText(path, _encoding);
        }

        /// <summary>
        /// 実行するSQLを取得します。
        /// </summary>
        /// <param name="sqlText">SQL文字列</param>
        /// <param name="paramList">パラメータリスト</param>
        /// <returns>SQL</returns>
        public static string GetExecuteSql(string sqlText, IList<IDbDataParameter> paramList)
        {
            // SQLテキスト内の置換文字列を取得します。
            string oldText = getRepalceText(sqlText);

            if (string.IsNullOrEmpty(oldText))
            {
                // 置換文字列が存在しないので、SQL文字列を実行SQLとして返します。
                return sqlText;
            }
            else
            {
                // パラメータリスト内の、番号付パラメータの個数を取得します。
                int count = getNumberingParameterCount(paramList);

                // 置換文字列内のパラメータ文字列リストを取得します。
                IList<string> paramStringList = getParameterNameList(oldText);

                StringBuilder newText = new StringBuilder();

                newText.Append("(");

                for (int i = 0; i < count; i++)
                {
                    newText.Append("(");

                    string text = oldText.Replace("{", "").Replace("}", "");

                    foreach (string paramString in paramStringList)
                    {
                        Regex r = new Regex(string.Format("{0}\\s|{0}$", paramString));
                        text = r.Replace(text, string.Format(ApplicationConst.NumberingSqlParameterFormat + " ", paramString, i.ToString()));
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

        private static string toSqlFileName(string sqlName)
        {
            return sqlName.Substring(0, sqlName.Length - 3);
        }

        // パラメータリスト内の、番号付パラメータの個数を取得します。
        private static int getNumberingParameterCount(IList<IDbDataParameter> paramList)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            foreach (IDbDataParameter param in paramList)
            {
                if (_numberingParameterNameRegex.IsMatch(param.ParameterName))
                {
                    string paramterName = _numberingParameterNameRegex.Replace(param.ParameterName, "");
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
        private static string getRepalceText(string sqlText)
        {
            Match m = _replaceRegex.Match(sqlText);

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
        private static IList<string> getParameterNameList(string replaceText)
        {
            List<string> list = new List<string>();

            Match m = _parameterRegex.Match(replaceText);

            while (m.Success)
            {
                list.Add(m.Value);
                m = m.NextMatch();
            }

            return list;
        }
    }
}
