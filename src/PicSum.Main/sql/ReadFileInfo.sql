SELECT mf.file_path
      ,COALESCE(tr.rating, 0) AS rating
  FROM m_file mf
       LEFT JOIN t_rating tr
         ON tr.file_id = mf.file_id
 WHERE mf.file_path = :file_path