INSERT INTO t_rating (
     file_id
    ,rating
    ,registration_date
)
SELECT mf.file_id
      ,:rating
      ,:registration_date
  FROM m_file mf
 WHERE mf.file_path = :file_path