using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Connection
{
    /// <summary>
    /// thumb.sqlite コネクション
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class ThumbnailDB(string dbFilePath)
        : AbstractDatabase(dbFilePath, TABLE_CREATE_SQL, true), IThumbnailDB
    {
        private const string TABLE_CREATE_SQL =
        @"
/* サムネイルIDM */
CREATE TABLE 'm_thumbnail_id' (
      'thumbnail_id' INTEGER NOT NULL
     ,'create_date'  DATETIME
     ,'update_date'  DATETIME
);

/* サムネイルT */
CREATE TABLE 't_thumbnail' (
      'file_path'             TEXT(256) NOT NULL
     ,'thumbnail_id'          INTEGER   NOT NULL
     ,'thumbnail_start_point' INTEGER   NOT NULL
     ,'thumbnail_size'        INTEGER   NOT NULL
     ,'thumbnail_width'       INTEGER   NOT NULL
     ,'thumbnail_height'      INTEGER   NOT NULL
     ,'source_width'          INTEGER   NOT NULL
     ,'source_height'         INTEGER   NOT NULL
     ,'file_update_date'      DATETIME  NOT NULL
     ,'create_date'           DATETIME
     ,'update_date'           DATETIME
     ,PRIMARY KEY (
          'file_path'
      )
);

/* サムネイルIDM UPDATE */
CREATE TRIGGER m_thumbnail_id_update_trigger
    AFTER UPDATE
       ON m_thumbnail_id
    BEGIN UPDATE m_thumbnail_id
             SET update_date = DATETIME('NOW', 'LOCALTIME')
           WHERE thumbnail_id = NEW.thumbnail_id;
   END;

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

/* サムネイルファイルIDM規定値挿入 */
INSERT INTO m_thumbnail_id (
     thumbnail_id
    ,create_date
) VALUES (
     0
    ,DATETIME('NOW', 'LOCALTIME')
);
        ";
    }
}
