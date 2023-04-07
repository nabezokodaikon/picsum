SELECT mf.file_path
	FROM m_file mf
       INNER JOIN (SELECT tfvh.file_id
                         ,COUNT(1) AS cnt
                     FROM t_folder_view_history tfvh
                    GROUP BY tfvh.file_id
                  ) t
          ON t.file_id = mf.file_id
ORDER BY t.cnt DESC
LIMIT 200;