SELECT mf.file_path
      ,tt.tag
      ,tt.registration_date
  FROM m_file mf
       INNER JOIN t_tag tt
         ON tt.file_id = mf.file_id
 WHERE tt.tag = :tag
 ORDER BY tt.registration_date DESC