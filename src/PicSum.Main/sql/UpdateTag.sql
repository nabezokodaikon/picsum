UPDATE t_tag
   SET tag = :tag
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
   AND tag = :tag