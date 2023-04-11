SELECT mf.file_path
  FROM m_file mf
       INNER JOIN (SELECT tfvc.file_id
                         ,tfvc.view_count AS cnt
                     FROM t_folder_view_counter tfvc
                 ORDER BY tfvc.view_count DESC
                    LIMIT 200
                  ) t
          ON t.file_id = mf.file_id