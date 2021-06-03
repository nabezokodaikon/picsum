UPDATE t_folder_state
   SET sort_type_id       = :sort_type_id
      ,is_ascending       = :is_ascending
      ,selected_file_path = :selected_file_path
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :folder_path
                 )