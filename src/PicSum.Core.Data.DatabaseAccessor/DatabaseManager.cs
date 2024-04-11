using System;
using System.Collections.Generic;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// DB管理
    /// </summary>
    public static class DatabaseManager<TConnection>
        where TConnection : ConnectionBase
    {
        // DBコネクション
        private static TConnection connection = null;

        /// <summary>
        /// DBファイルパス。
        /// </summary>
        public static string DBFilePath
        {
            get
            {
                return connection.DBFilePath;
            }
        }

        /// <summary>
        /// DBに接続します。
        /// </summary>
        /// <param name="connection"></param>
        public static void Connect(TConnection connection)
        {
            DatabaseManager<TConnection>.connection =
                connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// DBをクローズします。
        /// </summary>
        public static void Close()
        {
            DatabaseManager<TConnection>.connection.Dispose();
        }

        /// <summary>
        /// トランザクションを開始します。
        /// </summary>
        /// <returns>トランザクションオブジェクト</returns>
        public static Transaction BeginTransaction()
        {
            return DatabaseManager<TConnection>.connection.BeginTransaction();
        }

        /// <summary>
        /// SQLを実行し、更新します。
        /// </summary>
        /// <param name="sql">データアクセサ</param>
        /// <returns>更新されたレコードが存在するならTrue。存在しなければFalseを返します。</returns>
        public static bool Update(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            return DatabaseManager<TConnection>.connection.Update(sql);
        }

        /// <summary>
        /// SQLを実行し、リストを取得します。
        /// </summary>
        /// <typeparam name="TDto">戻り値のDto</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dtoリスト</returns>
        public static IList<TDto> ReadList<TDto>(SqlBase<TDto> sql) where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            return DatabaseManager<TConnection>.connection.ReadList<TDto>(sql);
        }

        /// <summary>
        /// SQLを実行し、1レコード取得します。
        /// </summary>
        /// <typeparam name="TDto">戻り値のDto型</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>Dto</returns>
        public static TDto ReadLine<TDto>(SqlBase<TDto> sql) where TDto : IDto, new()
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            return DatabaseManager<TConnection>.connection.ReadLine<TDto>(sql);
        }

        /// <summary>
        /// SQLを実行し、値を取得します。
        /// </summary>
        /// <typeparam name="TDto">戻り値の型</typeparam>
        /// <param name="sql">データアクセサ</param>
        /// <returns>1オブジェクトの実行結果</returns>
        public static T ReadValue<T>(SqlBase sql)
        {
            ArgumentNullException.ThrowIfNull(sql, nameof(sql));

            return (T)DatabaseManager<TConnection>.connection.ReadValue<T>(sql);
        }
    }
}
