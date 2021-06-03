INSERT INTO m_file (
     file_id
    ,file_path
)
SELECT mfi.file_id
      ,:file_path
  FROM m_file_id mfi