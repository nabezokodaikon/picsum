INSERT INTO t_rating (
     file_id
    ,rating
)
SELECT mf.file_id
      ,:rating
  FROM m_file mf
 WHERE mf.file_path = :file_path