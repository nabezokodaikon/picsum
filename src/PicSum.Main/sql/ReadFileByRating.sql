SELECT mf.file_path
  FROM m_file mf
       INNER JOIN t_rating tr
          ON tr.file_id = mf.file_id
 WHERE tr.rating = :rating
 ORDER BY tr.update_date DESC
