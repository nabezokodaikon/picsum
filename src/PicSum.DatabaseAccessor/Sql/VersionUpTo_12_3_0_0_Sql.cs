using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{

    public sealed class VersionUpTo_12_3_0_0_Sql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
DROP TRIGGER t_directory_view_history_insert_trigger;
DROP TRIGGER t_directory_view_history_update_trigger;

DROP TABLE t_directory_view_history;

CREATE TABLE 't_directory_view_history' (
     'file_id'         INTEGER  NOT NULL
    ,'view_date_ticks' INTEGER  NOT NULL
    ,'create_date'     DATETIME
    ,'update_date'     DATETIME
    ,PRIMARY KEY (
         'file_id'
     )
);

CREATE TRIGGER t_directory_view_history_insert_trigger
    AFTER INSERT
       ON t_directory_view_history
    BEGIN UPDATE t_directory_view_history
             SET create_date     = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id         = NEW.file_id;
   END;

CREATE TRIGGER t_directory_view_history_update_trigger
    AFTER UPDATE
       ON t_directory_view_history
    BEGIN UPDATE t_directory_view_history
             SET update_date     = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id         = NEW.file_id;
   END;
";
        public VersionUpTo_12_3_0_0_Sql()
            : base(SQL_TEXT)
        {

        }
    }
}
