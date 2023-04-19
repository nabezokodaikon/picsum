SELECT mf.file_path
      ,tr.registration_date
  FROM m_file mf
       INNER JOIN t_rating tr
          ON tr.file_id = mf.file_id
 WHERE tr.rating = :rating
 ORDER BY tr.registration_date DESC
