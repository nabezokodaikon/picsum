using SWF.Core.Base;
using System.Data;
using System.Data.SQLite;
using System.Runtime.Versioning;

namespace SWF.Core.DatabaseAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class SqlParameterUtil
    {
        public static IDbDataParameter CreateParameter(string paramName, string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

            return SqlParameterUtil.CreateParameter(paramName, value, DbType.String);
        }

        public static IDbDataParameter CreateParameter(string paramName, string value, int size)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

            return SqlParameterUtil.CreateParameter(paramName, value, size, DbType.String);
        }

        public static IDbDataParameter CreateParameter(string paramName, int value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

            return SqlParameterUtil.CreateParameter(paramName, value, DbType.Int32);
        }

        public static IDbDataParameter CreateParameter(string paramName, long value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

            return SqlParameterUtil.CreateParameter(paramName, value, DbType.Int64);
        }

        public static IDbDataParameter CreateParameter(string paramName, bool value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

            return SqlParameterUtil.CreateParameter(paramName, value, DbType.Boolean);
        }

        public static IDbDataParameter CreateParameter(string paramName, DateTime value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

            return SqlParameterUtil.CreateParameter(paramName, value, DbType.DateTime);
        }

        public static IDbDataParameter CreateParameter(string paramName, byte[] value)
        {
            ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            return SqlParameterUtil.CreateParameter(paramName, value, DbType.Binary);
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
                var param = string.Format(AppConstants.NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString());
                var value = valueList[i];
                list.Add(SqlParameterUtil.CreateParameter(param, value, DbType.String));
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
                var param = string.Format(AppConstants.NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString());
                var value = valueList[i];
                list.Add(SqlParameterUtil.CreateParameter(param, value, size, DbType.String));
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
                var param = string.Format(AppConstants.NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString());
                var value = valueList[i];
                list.Add(SqlParameterUtil.CreateParameter(param, value, DbType.Int32));
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
                var param = string.Format(AppConstants.NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString());
                var value = valueList[i];
                list.Add(SqlParameterUtil.CreateParameter(param, value, DbType.Boolean));
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
                var param = string.Format(AppConstants.NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString());
                var value = valueList[i];
                list.Add(SqlParameterUtil.CreateParameter(param, value, DbType.DateTime));
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
                var param = string.Format(AppConstants.NUMBERING_SQL_PARAMETER_FORMAT, paramName, i.ToString());
                var value = valueList[i];
                list.Add(SqlParameterUtil.CreateParameter(param, value, DbType.Binary));
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
