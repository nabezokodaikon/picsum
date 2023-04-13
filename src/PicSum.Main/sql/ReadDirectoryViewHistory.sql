SELECT mf.file_path
      ,tfvh.view_date
  FROM m_file mf
       INNER JOIN t_directory_view_history tfvh
          ON tfvh.file_id = mf.file_id       
 ORDER BY tfvh.view_date DESC
 LIMIT :limit