using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Connection
{
    /// <summary>
    /// fileinfo.sqlite コネクション
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class FileInfoDB(string dbFilePath)
        : AbstractDatabase(dbFilePath, TABLE_CREATE_SQL, false), IFileInfoDB
    {
        private const string TABLE_CREATE_SQL =
        @"
/* ファイルIDM */
CREATE TABLE 'm_file_id' (
      'file_id'     INTEGER  NOT NULL
     ,'create_date' DATETIME
     ,'update_date' DATETIME
     ,PRIMARY KEY (
          'file_id'
      )
);

/* ファイルM */
CREATE TABLE 'm_file' (
      'file_id'     INTEGER   NOT NULL
     ,'file_path'   TEXT(256) NOT NULL
     ,'create_date' DATETIME
     ,'update_date' DATETIME
     ,PRIMARY KEY (
          'file_id'
      )
     ,UNIQUE (
          'file_path'
     )
);

/* タグT */
CREATE TABLE 't_tag' (
     'file_id'           INTEGER  NOT NULL
    ,'tag'               TEXT(64) NOT NULL
    ,'registration_date' DATETIME NOT NULL
    ,'create_date'       DATETIME
    ,'update_date'       DATETIME
    ,PRIMARY KEY (
         'file_id'
        ,'tag'
     )
);

/* 評価値T */
CREATE TABLE 't_rating' (
     'file_id'           INTEGER  NOT NULL
    ,'rating'            INTEGER  NOT NULL
    ,'registration_date' DATETIME NOT NULL
    ,'create_date'       DATETIME
    ,'update_date'       DATETIME
    ,PRIMARY KEY (
         'file_id'
        ,'rating'
     )
);

/* ディレクトリ表示回数T */
CREATE TABLE 't_directory_view_counter' (
     'file_id'         INTEGER  NOT NULL
    ,'view_count'      INTEGER  NOT NULL
    ,'create_date'     DATETIME
    ,'update_date'     DATETIME
    ,PRIMARY KEY (
         'file_id'
     )
);

/* ディレクトリ表示履歴T */
CREATE TABLE 't_directory_view_history' (
     'file_id'         INTEGER  NOT NULL
    ,'file_history_id' INTEGER  NOT NULL
    ,'view_date'       DATETIME NOT NULL
    ,'create_date'     DATETIME
    ,'update_date'     DATETIME
    ,PRIMARY KEY (
         'file_id'
        ,'file_history_id'
     )
);

/* ディレクトリ状態T */
CREATE TABLE 't_directory_state' (
     'file_id'            INTEGER  NOT NULL
    ,'sort_type_id'       INTEGER  NOT NULL
    ,'is_ascending'       BOOLEAN  NOT NULL
    ,'selected_file_path' TEXT
    ,'create_date'        DATETIME
    ,'update_date'        DATETIME
    ,PRIMARY KEY (
         'file_id'
     )
);

/* ブックマークT */
CREATE TABLE 't_bookmark' (
     'file_id'           INTEGER  NOT NULL
    ,'registration_date' DATETIME NOT NULL
    ,'create_date'       DATETIME
    ,'update_date'       DATETIME
    ,PRIMARY KEY (
         'file_id'
     )
);

/* ファイルIDM UPDATE */
CREATE TRIGGER m_file_id_update_trigger
    AFTER UPDATE
       ON m_file_id
    BEGIN UPDATE m_file_id
             SET update_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id;
   END;

/* ファイルM INSERT */
CREATE TRIGGER m_file_insert_trigger
    AFTER INSERT
       ON m_file
    BEGIN UPDATE m_file
             SET create_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id;
   END;

/* ファイルM UPDATE */
CREATE TRIGGER m_file_update_trigger
    AFTER UPDATE
       ON m_file
    BEGIN UPDATE m_file
             SET update_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id;
   END;

/* タグT INSERT */
CREATE TRIGGER t_tag_insert_trigger
    AFTER INSERT
       ON t_tag
    BEGIN UPDATE t_tag
             SET create_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id
             AND tag     = NEW.tag;
   END;

/* タグT UPDATE */
CREATE TRIGGER t_tag_update_trigger
    AFTER UPDATE
       ON t_tag
    BEGIN UPDATE t_tag
             SET update_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id
             AND tag     = NEW.tag;
   END;

/* 評価値T INSERT */
CREATE TRIGGER t_rating_insert_trigger
    AFTER INSERT
       ON t_rating
    BEGIN UPDATE t_rating
             SET create_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id;
   END;

/* 評価値T UPDATE */
CREATE TRIGGER t_rating_update_trigger
    AFTER UPDATE
       ON t_rating
    BEGIN UPDATE t_rating
             SET update_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id;
   END;

/* ディレクトリ表示回数T INSERT */
CREATE TRIGGER t_directory_view_counter_insert_trigger
    AFTER INSERT
       ON t_directory_view_counter
    BEGIN UPDATE t_directory_view_counter
             SET create_date     = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id         = NEW.file_id;
   END;

/* ディレクトリ表示回数T UPDATE */
CREATE TRIGGER t_directory_view_counter_update_trigger
    AFTER UPDATE
       ON t_directory_view_counter
    BEGIN UPDATE t_directory_view_counter
             SET update_date     = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id         = NEW.file_id;
   END;

/* ディレクトリ表示履歴T INSERT */
CREATE TRIGGER t_directory_view_history_insert_trigger
    AFTER INSERT
       ON t_directory_view_history
    BEGIN UPDATE t_directory_view_history
             SET create_date     = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id         = NEW.file_id
             AND file_history_id = NEW.file_history_id;
   END;

/* ディレクトリ表示履歴T UPDATE */
CREATE TRIGGER t_directory_view_history_update_trigger
    AFTER UPDATE
       ON t_directory_view_history
    BEGIN UPDATE t_directory_view_history
             SET update_date     = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id         = NEW.file_id
             AND file_history_id = NEW.file_history_id;
   END;

/* ディレクトリ状態T INSERT */
CREATE TRIGGER t_directory_state_insert_trigger
    AFTER INSERT
       ON t_directory_state
    BEGIN UPDATE t_directory_state
             SET create_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id     = NEW.file_id;
   END;

/* ディレクトリ状態T UPDATE */
CREATE TRIGGER t_directory_state_update_trigger
    AFTER UPDATE
       ON t_directory_state
    BEGIN UPDATE t_directory_state
             SET update_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id     = NEW.file_id;
   END;

/* ブックマークT INSERT */
CREATE TRIGGER t_bookmark_insert_trigger
    AFTER INSERT
       ON t_bookmark
    BEGIN UPDATE t_bookmark
             SET create_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id;
   END;

/* ブックマークT UPDATE */
CREATE TRIGGER t_bookmark_update_trigger
    AFTER UPDATE
       ON t_bookmark
    BEGIN UPDATE t_bookmark
             SET update_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id;
   END;

/* ファイルM挿入時に、ファイルMID.ファイルIDをインクリメントします。*/
CREATE TRIGGER increment_file_id
    AFTER INSERT
       ON m_file
    BEGIN UPDATE m_file_id
             SET file_id = file_id + 1;
   END;

/* ファイルIDM規定値挿入 */
INSERT INTO m_file_id (
     file_id
    ,create_date
) VALUES (
     1
    ,DATETIME('NOW', 'LOCALTIME')
);
        ";
    }
}
