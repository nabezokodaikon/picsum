SELECT mf.file_path
      ,tt.tag
  FROM m_file mf
       INNER JOIN t_tag tt
         ON tt.file_id = mf.file_id