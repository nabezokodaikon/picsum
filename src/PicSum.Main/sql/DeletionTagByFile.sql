DELETE FROM t_tag
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )