SELECT mf.file_path
      ,COALESCE(tr.rating, 0) AS rating
      ,COALESCE(tfvh.cnt, 0) AS view_count
      ,tfvh.view_date AS last_view_date
  FROM m_file mf
       LEFT JOIN t_rating tr
         ON tr.file_id = mf.file_id
       LEFT JOIN ( SELECT file_id
                         ,COUNT(1) AS cnt
                         ,MAX(view_date) AS view_date
                     FROM t_file_view_history
                    GROUP BY file_id
                    UNION
                    SELECT file_id
                          ,COUNT(1)
                          ,MAX(view_date)
                     FROM t_folder_view_history
                    GROUP BY file_id
                 ) tfvh
         ON tfvh.file_id = mf.file_id
 WHERE mf.file_path = :file_path