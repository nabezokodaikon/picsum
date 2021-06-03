SELECT mf.file_path
  FROM m_file mf
       INNER JOIN t_folder_view_history tfvh
          ON tfvh.file_id = mf.file_id
 GROUP BY mf.file_path          
 ORDER BY tfvh.view_date DESC
 LIMIT :limit