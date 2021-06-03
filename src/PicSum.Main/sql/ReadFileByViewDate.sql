SELECT mf.file_path
  FROM m_file mf
       INNER JOIN t_file_view_history tfvh
          ON tfvh.file_id = mf.file_id
 WHERE STRFTIME('%Y%m%d', tfvh.view_date) = STRFTIME('%Y%m%d', :view_date)
 GROUP BY mf.file_path
 ORDER BY tfvh.view_date DESC