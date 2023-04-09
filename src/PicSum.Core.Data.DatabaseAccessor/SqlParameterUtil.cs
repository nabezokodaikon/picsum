using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using PicSum.Core.Base.Conf;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// SQLパラメータユーティリティ
    /// </summary>
    public static class SqlParameterUtil
    {
        #region パラメータ取得

        public static IDbDataParameter CreateParameter(string paramName, string value)
        {
            return CreateParameter(paramName, value, DbType.String);
        }

        public static IDbDataParameter CreateParameter(string paramName, string value, int size)
        {
            return CreateParameter(paramName, value, size, DbType.String);
        }

        public static IDbDataParameter CreateParameter(string paramName, int value)
        {
            return CreateParameter(paramName, value, DbType.Int32);
        }

        public static IDbDataParameter CreateParameter(string paramName, bool value)
        {
            return CreateParameter(paramName, value, DbType.Boolean);
        }

        public static IDbDataParameter CreateParameter(string paramName, DateTime value)
        {
            return CreateParameter(paramName, value, DbType.DateTime);
        }

        public static IDbDataParameter CreateParameter(string paramName, byte[] value)
        {
            return CreateParameter(paramName, value, DbType.Binary);
        }

        public static IDbDataParameter CreateParameter(string paramName, DBNull value)
        {
            return new SQLiteParameter(paramName, value);
        }

        public static IList<IDbDataParameter> CreateParameter(string paramName, IList<string> valueList)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();

            for (int i = 0; i < valueList.Count; i++)
            {
                string param = string.Format(ApplicationConst.NumberingSqlParameterFormat, paramName, i.ToString());
                string value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.String));
            }

            return list;
        }

        public static IList<IDbDataParameter> CreateParameter(string paramName, IList<string> valueList, int size)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();

            for (int i = 0; i < valueList.Count; i++)
            {
                string param = string.Format(ApplicationConst.NumberingSqlParameterFormat, paramName, i.ToString());
                string value = valueList[i];
                list.Add(CreateParameter(param, value, size, DbType.String));
            }

            return list;
        }

        public static IList<IDbDataParameter> CreateParameter(string paramName, IList<int> valueList)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();

            for (int i = 0; i < valueList.Count; i++)
            {
                string param = string.Format(ApplicationConst.NumberingSqlParameterFormat, paramName, i.ToString());
                int value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.Int32));
            }

            return list;
        }

        public static IList<IDbDataParameter> CreateParameter(string paramName, IList<bool> valueList)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();

            for (int i = 0; i < valueList.Count; i++)
            {
                string param = string.Format(ApplicationConst.NumberingSqlParameterFormat, paramName, i.ToString());
                bool value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.Boolean));
            }

            return list;
        }

        public static IList<IDbDataParameter> CreateParameter(string paramName, IList<DateTime> valueList)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();

            for (int i = 0; i < valueList.Count; i++)
            {
                string param = string.Format(ApplicationConst.NumberingSqlParameterFormat, paramName, i.ToString());
                DateTime value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.DateTime));
            }

            return list;
        }

        public static IList<IDbDataParameter> CreateParameter(string paramName, IList<byte[]> valueList)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();

            for (int i = 0; i < valueList.Count; i++)
            {
                string param = string.Format(ApplicationConst.NumberingSqlParameterFormat, paramName, i.ToString());
                byte[] value = valueList[i];
                list.Add(CreateParameter(param, value, DbType.Binary));
            }

            return list;
        }

        private static SQLiteParameter CreateParameter(string paramName, object value, DbType dbType)
        {
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
            var param = new SQLiteParameter
            {
                ParameterName = paramName,
                Value = value,
                Size = size,
                DbType = dbType
            };

            return param;
        }

        #endregion
    }
}
