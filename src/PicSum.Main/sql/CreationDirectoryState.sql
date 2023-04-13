INSERT INTO t_directory_state (
     file_id
    ,sort_type_id
    ,is_ascending
    ,selected_file_path
)
SELECT mf.file_id
      ,:sort_type_id
      ,:is_ascending
      ,:selected_file_path
  FROM m_file mf
 WHERE mf.file_path = :directory_path