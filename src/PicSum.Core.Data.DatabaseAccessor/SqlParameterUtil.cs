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
            return CreateParameter(paramName, value, getDbType(value));
        }

        public static IDbDataParameter CreateParameter(string paramName, string value, int size)
        {
            return CreateParameter(paramName, value, size, getDbType(value));
        }

        public static IDbDataParameter CreateParameter(string paramName, int value)
        {
            return CreateParameter(paramName, value, getDbType(value));
        }

        public static IDbDataParameter CreateParameter(string paramName, bool value)
        {
            return CreateParameter(paramName, value, getDbType(value));
        }

        public static IDbDataParameter CreateParameter(string paramName, DateTime value)
        {
            return CreateParameter(paramName, value, getDbType(value));
        }

        public static IDbDataParameter CreateParameter(string paramName, byte[] value)
        {
            return CreateParameter(paramName, value, getDbType(value));
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
                list.Add(CreateParameter(param, value, getDbType(value)));
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
                list.Add(CreateParameter(param, value, size, getDbType(value)));
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
                list.Add(CreateParameter(param, value, getDbType(value)));
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
                list.Add(CreateParameter(param, value, getDbType(value)));
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
                list.Add(CreateParameter(param, value, getDbType(value)));
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
                list.Add(CreateParameter(param, value, getDbType(value)));
            }

            return list;
        }

        private static SQLiteParameter CreateParameter(string paramName, object value, DbType dbType)
        {
            SQLiteParameter param = new SQLiteParameter();

            param.ParameterName = paramName;
            param.Value = value;
            param.DbType = dbType;

            return param;
        }

        private static SQLiteParameter CreateParameter(string paramName, object value, int size, DbType dbType)
        {
            SQLiteParameter param = new SQLiteParameter();

            param.ParameterName = paramName;
            param.Value = value;
            param.Size = size;
            param.DbType = dbType;

            return param;
        }

        #endregion

        #region DbType取得

        private static DbType getDbType(string value)
        {
            return DbType.String;
        }

        private static DbType getDbType(int value)
        {
            return DbType.Int32;
        }

        private static DbType getDbType(bool value)
        {
            return DbType.Boolean;
        }

        private static DbType getDbType(DateTime value)
        {
            return DbType.DateTime;
        }

        private static DbType getDbType(byte[] value)
        {
            return DbType.Binary;
        }

        #endregion
    }
}
