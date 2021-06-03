INSERT INTO t_folder_view_history (
     file_id
    ,file_history_id
    ,view_date
)
SELECT mf.file_id
      ,( SELECT COUNT(1)
           FROM t_folder_view_history tfvh
          WHERE tfvh.file_id = mf.file_id
       )
      ,DATETIME('NOW', 'LOCALTIME')
  FROM m_file mf
 WHERE mf.file_path = :folder_path