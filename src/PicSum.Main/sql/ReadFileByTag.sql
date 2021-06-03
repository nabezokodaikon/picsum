SELECT mf.file_path
  FROM m_file mf
       LEFT JOIN t_tag tt
         ON tt.file_id = mf.file_id
 WHERE tt.tag = :tag
 ORDER BY tt.update_date DESC