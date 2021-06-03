SELECT mf1.file_path AS folder_path
      ,tfs.sort_type_id
      ,tfs.is_ascending
      ,tfs.selected_file_path
  FROM m_file mf1
       INNER JOIN t_folder_state tfs
          ON tfs.file_id = mf1.file_id
 WHERE mf1.file_path = :folder_path