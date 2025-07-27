using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class VersionUpTo_12_2_1_0_Sql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
DROP TRIGGER t_rating_insert_trigger;
DROP TRIGGER t_rating_update_trigger;

CREATE TABLE 'temp' (
     'file_id'           INTEGER  NOT NULL
    ,'rating'            INTEGER  NOT NULL
    ,'registration_date' DATETIME NOT NULL
    ,'create_date'       DATETIME
    ,'update_date'       DATETIME
    ,PRIMARY KEY (
         'file_id'
     )
);

INSERT INTO temp (
    file_id
   ,rating
   ,registration_date
   ,create_date
   ,update_date
)
SELECT file_id
      ,rating
      ,registration_date
      ,create_date
      ,update_date
 FROM t_rating
WHERE rating == 1;

DROP TABLE t_rating;

CREATE TABLE 't_rating' (
     'file_id'           INTEGER  NOT NULL
    ,'rating'            INTEGER  NOT NULL
    ,'registration_date' DATETIME NOT NULL
    ,'create_date'       DATETIME
    ,'update_date'       DATETIME
    ,PRIMARY KEY (
         'file_id'
     )
);

INSERT INTO t_rating (
    file_id
   ,rating
   ,registration_date
   ,create_date
   ,update_date
)
SELECT file_id
      ,rating
      ,registration_date
      ,create_date
      ,update_date
 FROM temp;

DROP TABLE temp;

CREATE TRIGGER t_rating_insert_trigger
    AFTER INSERT
       ON t_rating
    BEGIN UPDATE t_rating
             SET create_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id;
   END;

CREATE TRIGGER t_rating_update_trigger
    AFTER UPDATE
       ON t_rating
    BEGIN UPDATE t_rating
             SET update_date = DATETIME('NOW', 'LOCALTIME')
           WHERE file_id = NEW.file_id;
   END;
";

        public VersionUpTo_12_2_1_0_Sql()
            : base(SQL_TEXT)
        {

        }
    }
}
