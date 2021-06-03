INSERT INTO t_tag (
     file_id
    ,tag
)
SELECT mf.file_id
      ,:tag
  FROM m_file mf
 WHERE mf.file_path = :file_path
      