DELETE FROM t_folder_state
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )