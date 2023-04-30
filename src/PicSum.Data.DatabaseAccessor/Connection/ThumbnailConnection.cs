using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Connection
{
    /// <summary>
    /// thumb.sqlite コネクション
    /// </summary>
    public sealed class ThumbnailConnection
        : ConnectionBase
    {
        private static string tableCreateSql =
        @"
/* サムネイルT */
CREATE TABLE 't_thumbnail' (
      'file_path'        TEXT(256)     NOT NULL
     ,'thumbnail_buffer' BLOB          NOT NULL
     ,'thumbnail_width'  INTEGER       NOT NULL
     ,'thumbnail_height' INTEGER       NOT NULL
     ,'source_width'     INTEGER       NOT NULL
     ,'source_height'    INTEGER       NOT NULL
     ,'file_update_date' DATETIME      NOT NULL
     ,'create_date'      DATETIME
     ,'update_date'      DATETIME
     ,PRIMARY KEY (
          'file_path'
      )
);

/* サムネイルT INSERT */
CREATE TRIGGER t_thumbnail_insert_trigger
    AFTER INSERT
       ON t_thumbnail
    BEGIN UPDATE t_thumbnail
             SET create_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_path = NEW.file_path;
   END;

/* サムネイルT UPDATE */
CREATE TRIGGER t_thumbnail_update_trigger
    AFTER UPDATE
       ON t_thumbnail
    BEGIN UPDATE t_thumbnail
             SET update_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_path= NEW.file_path;
   END;
        ";

        public ThumbnailConnection(string dbFilePath)
            : base(dbFilePath, tableCreateSql) { }
    }
}
